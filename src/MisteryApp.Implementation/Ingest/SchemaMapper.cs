using System.Globalization;
using System.Text.Json;
using CsvHelper;
using MisteryApp.Abstractions.Ingest;
using MisteryApp.Common.Ingest;

namespace MisteryApp.Implementation.Ingest;

public class SchemaMapper(IKernelInvoker kernelInvoker) : ISchemaMapper
{
    public virtual async Task<MappedRecord> MapAsync(RawFile raw, CancellationToken ct = default)
    {
        var transformations = new List<string>();
        var content = PrepareContent(raw, transformations);
        var prompt = BuildMapPrompt(content, raw.DetectedFormat, transformations);
        var response = await kernelInvoker.InvokePromptAsync(prompt, ct);
        return ParseMappedRecord(response, transformations);
    }

    internal virtual string PrepareContent(RawFile raw, List<string> transformations)
    {
        if (raw.DetectedFormat != IngestFormat.Csv)
            return raw.RawContent;

        transformations.Add("CSV parsed to structured JSON");
        return ParseCsvContent(raw.RawContent);
    }

    internal virtual string ParseCsvContent(string rawContent)
    {
        using var reader = new StringReader(rawContent);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<dynamic>().ToList();
        return JsonSerializer.Serialize(records);
    }

    private static string BuildMapPrompt(
        string content,
        IngestFormat format,
        IReadOnlyList<string> transformations)
    {
        var transformList = transformations.Count > 0
            ? string.Join(", ", transformations)
            : "none";

        return $"""
            Map the following content to a JSON object. Use these field names where applicable:
            id, timestamp, value, description, source, category.
            Format: {format}
            Transformations applied: {transformList}
            Content:
            {content}
            Respond with ONLY a valid JSON object, no explanation.
            """;
    }

    private static MappedRecord ParseMappedRecord(string response, List<string> transformations)
    {
        var json = JsonResponseParser.ExtractObject(response);
        using var doc = JsonDocument.Parse(json);
        var fields = doc.RootElement.EnumerateObject()
            .ToDictionary(p => p.Name, p => (object?)p.Value.Clone());
        return new MappedRecord(fields, transformations);
    }
}
