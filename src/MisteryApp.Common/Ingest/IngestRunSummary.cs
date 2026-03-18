namespace MisteryApp.Common.Ingest;

public record IngestRunSummary(
    int FilesProcessed,
    int RecordsInserted,
    int RecordsHealed,
    int RecordsRejected,
    IReadOnlyList<string> Errors);
