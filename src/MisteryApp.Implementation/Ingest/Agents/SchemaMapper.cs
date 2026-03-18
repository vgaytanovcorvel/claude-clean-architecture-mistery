using System.Globalization;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;
using MisteryApp.Abstractions.Ingest.Interfaces;
using MisteryApp.Abstractions.Ingest.Models;
using MisteryApp.Common.Enums;

namespace MisteryApp.Implementation.Ingest.Agents;

public class SchemaMapper(IChatAgentInvoker chatAgentInvoker) : ISchemaMapper
{
    private const string MapSystemPrompt = """
        You are a data schema mapper. Extract ALL structured records from the file content — one JSON object per row or log entry.
        Respond with a JSON array of objects, one element per row/entry, using these optional fields per object:
        id (string), timestamp (ISO 8601 string), value (number), description (string), source (string), category (string).
        Example: [{"id":"1","timestamp":"2024-01-15T10:00:00Z","value":42.5,"description":"sample","source":"file.csv","category":"A"}, {"id":"2","timestamp":"2024-01-15T11:00:00Z","value":7.0,"description":"other","source":"file.csv","category":"B"}]
        If the file contains only a single entry, still return a single-element array.
        """;

    public virtual async Task<IReadOnlyList<MappedRecord>> MapAsync(
        FileClassification classification,
        CancellationToken cancellationToken)
    {
        var content = classification.Format == FileFormat.Csv
            ? BuildCsvContext(classification.FilePath)
            : await File.ReadAllTextAsync(classification.FilePath, cancellationToken);

        var response = await chatAgentInvoker.InvokeChatAsync(
            MapSystemPrompt,
            $"Map this {classification.Format} file content to structured records:\n{content}",
            null,
            cancellationToken);

        return ParseMappedRecords(response);
    }

    private static string BuildCsvContext(string filePath)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true };
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, config);
        var records = csv.GetRecords<dynamic>().Take(50).ToList();
        return JsonSerializer.Serialize(records);
    }

    private static IReadOnlyList<MappedRecord> ParseMappedRecords(string json)
    {
        try
        {
            var clean = StripMarkdownFences(json);
            using var doc = JsonDocument.Parse(clean);
            var root = doc.RootElement;
            if (root.ValueKind == JsonValueKind.Array)
                return root.EnumerateArray()
                    .Where(e => e.ValueKind == JsonValueKind.Object)
                    .Select(ParseSingleRecord)
                    .ToList();
            if (root.ValueKind == JsonValueKind.Object)
                return [ParseSingleRecord(root)];
            return [];
        }
        catch (Exception) when (true)
        {
            return [];
        }
    }

    private static string StripMarkdownFences(string text)
    {
        var trimmed = text.Trim();
        if (!trimmed.StartsWith("```", StringComparison.Ordinal))
            return trimmed;
        var firstNewline = trimmed.IndexOf('\n');
        if (firstNewline < 0) return trimmed;
        var withoutOpening = trimmed[(firstNewline + 1)..];
        var closingFence = withoutOpening.LastIndexOf("```", StringComparison.Ordinal);
        return closingFence >= 0 ? withoutOpening[..closingFence].Trim() : withoutOpening.Trim();
    }

    private static MappedRecord ParseSingleRecord(JsonElement element) =>
        new(
            GetStringProperty(element, "id"),
            GetDateTimeProperty(element, "timestamp"),
            GetDecimalProperty(element, "value"),
            GetStringProperty(element, "description"),
            GetStringProperty(element, "source"),
            GetStringProperty(element, "category"));

    private static string? GetStringProperty(JsonElement element, string property)
    {
        if (element.TryGetProperty(property, out var p) && p.ValueKind == JsonValueKind.String)
            return p.GetString();
        return null;
    }

    private static decimal? GetDecimalProperty(JsonElement element, string property)
    {
        if (!element.TryGetProperty(property, out var p)) return null;
        if (p.ValueKind == JsonValueKind.Number && p.TryGetDecimal(out var d)) return d;
        if (p.ValueKind == JsonValueKind.String && decimal.TryParse(p.GetString(), out var dp)) return dp;
        return null;
    }

    private static DateTimeOffset? GetDateTimeProperty(JsonElement element, string property)
    {
        if (!element.TryGetProperty(property, out var p)) return null;
        if (p.ValueKind == JsonValueKind.String)
        {
            var s = p.GetString();
            if (s is not null && DateTimeOffset.TryParse(s, out var dt)) return dt;
        }
        return null;
    }
}
