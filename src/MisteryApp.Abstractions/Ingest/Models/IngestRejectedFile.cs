namespace MisteryApp.Abstractions.Ingest.Models;

public record IngestRejectedFile(
    Guid Id,
    Guid RunId,
    string FilePath,
    string RejectionReason,
    DateTimeOffset RejectedAt);
