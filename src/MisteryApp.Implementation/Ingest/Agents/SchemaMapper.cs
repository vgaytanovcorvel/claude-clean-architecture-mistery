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
        You are a data schema mapper. Extract a structured record from file content.
        Respond with JSON only using these optional fields:
        id (string), timestamp (ISO 8601 string), value (number), description (string), source (string), category (string).
        Example: {"id":"rec-1","timestamp":"2024-01-15T10:00:00Z","value":42.5,"description":"sample","source":"file.csv","category":"A"}
        """;

    public virtual async Task<MappedRecord> MapAsync(
        FileClassification classification,
        CancellationToken cancellationToken)
    {
        var content = classification.Format == FileFormat.Csv
            ? BuildCsvContext(classification.FilePath)
            : await File.ReadAllTextAsync(classification.FilePath, cancellationToken);

        var response = await chatAgentInvoker.InvokeChatAsync(
            MapSystemPrompt,
            $"Map this {classification.Format} file content to a structured record:\n{content}",
            null,
            cancellationToken);

        return ParseMappedRecord(response);
    }

    private static string BuildCsvContext(string filePath)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true };
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, config);
        var records = csv.GetRecords<dynamic>().Take(50).ToList();
        return JsonSerializer.Serialize(records);
    }

    private static MappedRecord ParseMappedRecord(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            return new MappedRecord(
                GetStringProperty(root, "id"),
                GetDateTimeProperty(root, "timestamp"),
                GetDecimalProperty(root, "value"),
                GetStringProperty(root, "description"),
                GetStringProperty(root, "source"),
                GetStringProperty(root, "category"));
        }
        catch (JsonException)
        {
            return new MappedRecord(null, null, null, null, null, null);
        }
    }

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
