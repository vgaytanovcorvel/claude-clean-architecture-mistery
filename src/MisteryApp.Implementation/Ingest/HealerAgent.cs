using System.Text.Json;
using MisteryApp.Abstractions.Ingest;
using MisteryApp.Common.Ingest;

namespace MisteryApp.Implementation.Ingest;

public class HealerAgent(IKernelInvoker kernelInvoker) : IHealerAgent
{
    public virtual async Task<HealedRecord?> HealAsync(
        MappedRecord record,
        IReadOnlyList<QualityIssue> issues,
        CancellationToken ct = default)
    {
        var prompt = BuildHealPrompt(record, issues);
        var response = await kernelInvoker.InvokePromptAsync(prompt, ct);
        return ParseHealedRecord(response, record);
    }

    private static string BuildHealPrompt(MappedRecord record, IReadOnlyList<QualityIssue> issues)
    {
        var fieldsJson = JsonSerializer.Serialize(record.Fields);
        var issuesJson = JsonSerializer.Serialize(
            issues.Select(i => new { i.FieldName, Kind = i.Kind.ToString(), i.Description }));

        return $"""
            Fix the quality issues in this record.
            Return a JSON object with:
            - fields: the corrected fields object
            - healingNotes: array of strings describing changes made
            - success: true if all issues are fixed, false if unfixable
            Original record:
            {fieldsJson}
            Issues to fix:
            {issuesJson}
            Respond with ONLY a valid JSON object.
            """;
    }

    private static HealedRecord? ParseHealedRecord(string response, MappedRecord original)
    {
        var json = JsonResponseParser.ExtractObject(response);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (!root.TryGetProperty("success", out var successEl) || !successEl.GetBoolean())
            return null;

        var fields = ParseFields(root);
        var notes = ParseNotes(root);
        return new HealedRecord(new MappedRecord(fields, original.AppliedTransformations), notes);
    }

    private static Dictionary<string, object?> ParseFields(JsonElement root)
    {
        if (!root.TryGetProperty("fields", out var fieldsEl))
            return new Dictionary<string, object?>();

        return fieldsEl.EnumerateObject()
            .ToDictionary(p => p.Name, p => (object?)p.Value.Clone());
    }

    private static List<string> ParseNotes(JsonElement root)
    {
        if (!root.TryGetProperty("healingNotes", out var notesEl))
            return [];

        return notesEl.EnumerateArray()
            .Select(el => el.GetString() ?? "")
            .ToList();
    }

}
