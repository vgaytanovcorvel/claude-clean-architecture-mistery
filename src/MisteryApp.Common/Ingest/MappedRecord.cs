namespace MisteryApp.Common.Ingest;

public record MappedRecord(
    IReadOnlyDictionary<string, object?> Fields,
    IReadOnlyList<string> AppliedTransformations);
