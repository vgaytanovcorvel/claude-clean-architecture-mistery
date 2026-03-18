using System.Text.Json;
using MisteryApp.Abstractions.Ingest.Interfaces;
using MisteryApp.Abstractions.Ingest.Models;
using MisteryApp.Abstractions.Models;
using MisteryApp.Common.Enums;

namespace MisteryApp.Implementation.Ingest.Agents;

public class HealerAgent(IChatAgentInvoker chatAgentInvoker) : IHealerAgent
{
    private const string HealSystemPrompt = """
        You are a data healer. Fix issues in data records.
        Respond with the corrected record as JSON with these optional fields:
        id, timestamp, value, description, source, category.
        If the issue cannot be fixed, respond with: {"cannotHeal":true,"reason":"explanation"}
        """;

    public virtual async Task<Result<MappedRecord>> HealAsync(
        QualityReport report,
        CancellationToken cancellationToken)
    {
        if (report.IssueType == QualityIssueType.None)
            return Result<MappedRecord>.Success(report.Record);

        var recordJson = JsonSerializer.Serialize(report.Record);
        var userMessage = $"Fix this record with issue '{report.IssueType}': {report.IssueDetail}\nRecord: {recordJson}";

        var proposedFix = await chatAgentInvoker.InvokeChatAsync(
            HealSystemPrompt, userMessage, null, cancellationToken);

        if (IsCannotHeal(proposedFix))
            return Result<MappedRecord>.Failure(ExtractReason(proposedFix));

        var validationResponse = await chatAgentInvoker.InvokeChatAsync(
            HealSystemPrompt,
            "Validate your fix is correct and consistent, then output the final corrected record JSON.",
            [("user", userMessage), ("assistant", proposedFix)],
            cancellationToken);

        if (IsCannotHeal(validationResponse))
            return Result<MappedRecord>.Failure(ExtractReason(validationResponse));

        var finalJson = string.IsNullOrWhiteSpace(validationResponse) ? proposedFix : validationResponse;
        return Result<MappedRecord>.Success(ParseMappedRecord(report.Record, finalJson));
    }

    private static bool IsCannotHeal(string response)
    {
        try
        {
            using var doc = JsonDocument.Parse(response);
            return doc.RootElement.TryGetProperty("cannotHeal", out var prop)
                && prop.GetBoolean();
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static string ExtractReason(string response)
    {
        try
        {
            using var doc = JsonDocument.Parse(response);
            if (doc.RootElement.TryGetProperty("reason", out var r) && r.ValueKind == JsonValueKind.String)
                return r.GetString() ?? "Cannot heal record.";
        }
        catch (JsonException) { }
        return "Cannot heal record.";
    }

    private static MappedRecord ParseMappedRecord(MappedRecord original, string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            return new MappedRecord(
                GetStringOrOriginal(root, "id", original.Id),
                GetDateTimeOrOriginal(root, "timestamp", original.Timestamp),
                GetDecimalOrOriginal(root, "value", original.Value),
                GetStringOrOriginal(root, "description", original.Description),
                GetStringOrOriginal(root, "source", original.Source),
                GetStringOrOriginal(root, "category", original.Category));
        }
        catch (JsonException)
        {
            return original;
        }
    }

    private static string? GetStringOrOriginal(JsonElement element, string property, string? original)
    {
        if (element.TryGetProperty(property, out var p) && p.ValueKind == JsonValueKind.String)
            return p.GetString();
        return original;
    }

    private static decimal? GetDecimalOrOriginal(JsonElement element, string property, decimal? original)
    {
        if (!element.TryGetProperty(property, out var p)) return original;
        if (p.ValueKind == JsonValueKind.Number && p.TryGetDecimal(out var d)) return d;
        return original;
    }

    private static DateTimeOffset? GetDateTimeOrOriginal(JsonElement element, string property, DateTimeOffset? original)
    {
        if (!element.TryGetProperty(property, out var p)) return original;
        if (p.ValueKind == JsonValueKind.String)
        {
            var s = p.GetString();
            if (s is not null && DateTimeOffset.TryParse(s, out var dt)) return dt;
        }
        return original;
    }
}
