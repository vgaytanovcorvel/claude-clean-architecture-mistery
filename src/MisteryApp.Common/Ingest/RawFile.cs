namespace MisteryApp.Common.Ingest;

public record RawFile(string FilePath, string RawContent, IngestFormat DetectedFormat);
