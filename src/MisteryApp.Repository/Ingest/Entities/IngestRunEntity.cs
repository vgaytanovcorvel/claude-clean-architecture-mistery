using MisteryApp.Common.Enums;

namespace MisteryApp.Repository.Ingest.Entities;

public class IngestRunEntity
{
    public Guid RunId { get; set; }
    public IngestRunStatus Status { get; set; }
    public string InputPath { get; set; } = string.Empty;
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public ICollection<IngestFileEntity> Files { get; set; } = [];
}
