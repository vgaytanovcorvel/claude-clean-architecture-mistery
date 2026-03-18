namespace MisteryApp.Repository.Ingest.Entities;

public class IngestFileEntity
{
    public Guid Id { get; set; }
    public Guid RunId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public DateTimeOffset ProcessedAt { get; set; }
    public IngestRunEntity Run { get; set; } = null!;
    public IngestRejectedFileEntity? RejectedFile { get; set; }
    public ICollection<MappedRecordEntity> MappedRecords { get; set; } = [];
}
