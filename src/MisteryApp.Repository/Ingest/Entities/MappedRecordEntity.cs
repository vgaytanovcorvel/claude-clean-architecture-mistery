namespace MisteryApp.Repository.Ingest.Entities;

public class MappedRecordEntity
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public string? ExternalId { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public decimal? Value { get; set; }
    public string? Description { get; set; }
    public string? Source { get; set; }
    public string? Category { get; set; }
    public DateTimeOffset IngestedAt { get; set; }
    public IngestFileEntity File { get; set; } = null!;
}
