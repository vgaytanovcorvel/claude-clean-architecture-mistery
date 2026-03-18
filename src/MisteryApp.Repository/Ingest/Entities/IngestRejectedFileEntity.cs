namespace MisteryApp.Repository.Ingest.Entities;

public class IngestRejectedFileEntity
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public string RejectionReason { get; set; } = string.Empty;
    public IngestFileEntity File { get; set; } = null!;
}
