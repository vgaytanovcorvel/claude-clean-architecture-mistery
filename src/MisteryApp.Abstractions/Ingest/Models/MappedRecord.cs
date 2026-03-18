namespace MisteryApp.Abstractions.Ingest.Models;

/// <summary>
/// Represents a record extracted from an ingested file and mapped to the canonical schema.
/// All fields are nullable because the LLM may not extract all fields from every file format —
/// a CSV row may lack a timestamp, an OCR scan may omit a numeric value, etc.
/// Consumers should treat absent fields as unknown rather than zero/empty.
/// </summary>
public record MappedRecord(
    string? Id,
    DateTimeOffset? Timestamp,
    decimal? Value,
    string? Description,
    string? Source,
    string? Category);
