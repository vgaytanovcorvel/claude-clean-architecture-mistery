using System.Text.Json;
using MisteryApp.Abstractions.Ingest.Interfaces;
using MisteryApp.Abstractions.Ingest.Models;
using MisteryApp.Common.Enums;

namespace MisteryApp.Implementation.Ingest.Agents;

public class QualityCritic(IChatAgentInvoker chatAgentInvoker) : IQualityCritic
{
    private const string ReviewSystemPrompt = """
        You are a data quality critic. Review a data record for issues.
        If you need file metadata to complete the review, include "REQUEST_METADATA" in your response.
        Otherwise respond with JSON: {"issueType":"None|NegativeValue|MissingRequired|Hallucination","issueDetail":"description or null","isAcceptable":true|false}
        """;

    public virtual async Task<QualityReport> ReviewAsync(
        MappedRecord record,
        string sourceFilePath,
        CancellationToken cancellationToken)
    {
        var recordJson = JsonSerializer.Serialize(record);
        var userMessage = $"Review this record:\n{recordJson}\nSource file: {sourceFilePath}";

        var response = await chatAgentInvoker.InvokeChatAsync(
            ReviewSystemPrompt, userMessage, null, cancellationToken);

        if (!response.Contains("REQUEST_METADATA", StringComparison.OrdinalIgnoreCase))
            return ParseQualityReport(record, response);

        var metadata = GetFileMetadata(sourceFilePath);
        var followUpResponse = await chatAgentInvoker.InvokeChatAsync(
            ReviewSystemPrompt,
            $"File metadata:\n{metadata}",
            [("user", userMessage), ("assistant", response)],
            cancellationToken);

        return ParseQualityReport(record, followUpResponse);
    }

    internal virtual string GetFileMetadata(string filePath)
    {
        if (!File.Exists(filePath))
            return "File not found.";
        var info = new FileInfo(filePath);
        return $"Name: {info.Name}, Size: {info.Length} bytes, LastModified: {info.LastWriteTimeUtc:O}";
    }

    private static QualityReport ParseQualityReport(MappedRecord record, string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var issueTypeStr = GetStringProperty(root, "issueType") ?? "None";
            var issueType = Enum.TryParse<QualityIssueType>(issueTypeStr, ignoreCase: true, out var it)
                ? it : QualityIssueType.None;
            var isAcceptable = root.TryGetProperty("isAcceptable", out var ia)
                && ia.ValueKind == JsonValueKind.True;
            return new QualityReport(record, issueType, GetStringProperty(root, "issueDetail"), isAcceptable);
        }
        catch (JsonException)
        {
            return new QualityReport(record, QualityIssueType.None, null, true);
        }
    }

    private static string? GetStringProperty(JsonElement element, string property)
    {
        if (element.TryGetProperty(property, out var p) && p.ValueKind == JsonValueKind.String)
            return p.GetString();
        return null;
    }
}
