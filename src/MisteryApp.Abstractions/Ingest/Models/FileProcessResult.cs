namespace MisteryApp.Abstractions.Ingest.Models;

/// <summary>
/// Represents the outcome of processing a single file through the ingest pipeline.
/// Invariants:
/// <list type="bullet">
///   <item><description>When <see cref="Succeeded"/> is <c>true</c>: <see cref="Record"/> is non-null and <see cref="RejectionReason"/> is null.</description></item>
///   <item><description>When <see cref="Succeeded"/> is <c>false</c>: <see cref="Record"/> is null and <see cref="RejectionReason"/> is non-null.</description></item>
/// </list>
/// </summary>
public record FileProcessResult(
    string FilePath,
    bool Succeeded,
    MappedRecord? Record,
    string? RejectionReason);
