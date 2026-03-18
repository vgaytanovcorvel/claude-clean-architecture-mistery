namespace MisteryApp.Repository.Entities;

public class IngestedRecordEntity
{
    public Guid Id { get; set; }
    public string SourceFile { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public string FieldsJson { get; set; } = string.Empty;
    public string? HealingNotes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid RunId { get; set; }
}
