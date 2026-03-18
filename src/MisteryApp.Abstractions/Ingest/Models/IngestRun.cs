using MisteryApp.Common.Enums;

namespace MisteryApp.Abstractions.Ingest.Models;

public record IngestRun(
    Guid RunId,
    IngestRunStatus Status,
    string InputPath,
    DateTimeOffset StartedAt,
    DateTimeOffset? CompletedAt,
    int TotalFiles,
    int ProcessedFiles,
    int RejectedFiles);
