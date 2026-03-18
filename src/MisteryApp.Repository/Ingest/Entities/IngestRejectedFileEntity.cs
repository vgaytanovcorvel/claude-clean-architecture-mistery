namespace MisteryApp.Repository.Ingest.Entities;

public class IngestRejectedFileEntity
{
    public Guid Id { get; set; }
    public Guid RunId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string RejectionReason { get; set; } = string.Empty;
    public DateTimeOffset RejectedAt { get; set; }
    public IngestRunEntity IngestRun { get; set; } = null!;
}
