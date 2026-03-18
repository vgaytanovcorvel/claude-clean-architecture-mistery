using MisteryApp.Common.Enums;

namespace MisteryApp.Abstractions.Ingest.Models;

public record QualityReport(
    MappedRecord Record,
    QualityIssueType IssueType,
    string? IssueDetail,
    bool IsAcceptable);
