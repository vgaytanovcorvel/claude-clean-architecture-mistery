using System.Text.Json;
using MisteryApp.Abstractions.Ingest;
using MisteryApp.Common.Ingest;

namespace MisteryApp.Implementation.Ingest;

public class QualityCritic(IKernelInvoker kernelInvoker) : IQualityCritic
{
    public virtual async Task<IReadOnlyList<QualityIssue>> CritiqueAsync(
        MappedRecord record,
        CancellationToken ct = default)
    {
        var prompt = BuildCritiquePrompt(record);
        var response = await kernelInvoker.InvokePromptAsync(prompt, ct);
        return ParseIssues(response);
    }

    private static string BuildCritiquePrompt(MappedRecord record)
    {
        var fieldsJson = JsonSerializer.Serialize(record.Fields);
        var kinds = string.Join("|", Enum.GetNames<QualityIssueKind>());
        return $"""
            Review this mapped record for quality issues. Return a JSON array of issues.
            Each issue must have: fieldName (string), kind ({kinds}), description (string).
            Return [] if no issues found.
            Record:
            {fieldsJson}
            """;
    }

    private static IReadOnlyList<QualityIssue> ParseIssues(string response)
    {
        var json = JsonResponseParser.ExtractArray(response);
        var items = JsonSerializer.Deserialize<JsonElement[]>(json) ?? [];
        return items.Select(ParseIssue).ToList();
    }

    private static QualityIssue ParseIssue(JsonElement element)
    {
        var fieldName = element.TryGetProperty("fieldName", out var fn) ? fn.GetString() ?? "" : "";
        var kindStr = element.TryGetProperty("kind", out var k) ? k.GetString() ?? "" : "";
        var description = element.TryGetProperty("description", out var d) ? d.GetString() ?? "" : "";
        var kind = Enum.TryParse<QualityIssueKind>(kindStr, out var parsed) ? parsed : QualityIssueKind.Other;
        return new QualityIssue(fieldName, kind, description);
    }

}
