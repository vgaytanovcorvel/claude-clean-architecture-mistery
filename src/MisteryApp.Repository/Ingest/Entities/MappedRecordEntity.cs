namespace MisteryApp.Repository.Ingest.Entities;

public class MappedRecordEntity
{
    public Guid Id { get; set; }
    public Guid RunId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string? ExternalId { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public decimal? Value { get; set; }
    public string? Description { get; set; }
    public string? Source { get; set; }
    public string? Category { get; set; }
    public DateTimeOffset IngestedAt { get; set; }

    public IngestRunEntity Run { get; set; } = null!;
}
