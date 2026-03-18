namespace MisteryApp.Common.Ingest;

public record QualityIssue(string FieldName, QualityIssueKind Kind, string Description);
