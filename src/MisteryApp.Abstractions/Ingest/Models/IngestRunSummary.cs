using MisteryApp.Common.Enums;

namespace MisteryApp.Abstractions.Ingest.Models;

public record IngestRunSummary(
    Guid RunId,
    IngestRunStatus Status,
    string InputPath,
    DateTimeOffset StartedAt,
    DateTimeOffset? CompletedAt,
    int TotalFiles,
    int ProcessedFiles,
    int RejectedFiles);
