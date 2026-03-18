using MisteryApp.Common.Enums;

namespace MisteryApp.Repository.Ingest.Entities;

public class IngestRunEntity
{
    public Guid RunId { get; set; }
    public IngestRunStatus Status { get; set; }
    public string InputPath { get; set; } = string.Empty;
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public int TotalFiles { get; set; }
    public int ProcessedFiles { get; set; }
    public int RejectedFiles { get; set; }
    public ICollection<IngestRejectedFileEntity> RejectedFileEntities { get; set; } = [];
}
