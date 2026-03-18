namespace MisteryApp.Abstractions.Ingest.Models;

/// <summary>
/// Represents the outcome of processing a single file through the ingest pipeline.
/// Invariants:
/// <list type="bullet">
///   <item><description>When <see cref="Succeeded"/> is <c>true</c>: <see cref="Records"/> is non-null and non-empty, and <see cref="RejectionReason"/> is null.</description></item>
///   <item><description>When <see cref="Succeeded"/> is <c>false</c>: <see cref="Records"/> is null or empty, and <see cref="RejectionReason"/> is non-null.</description></item>
/// </list>
/// </summary>
public record FileProcessResult(
    string FilePath,
    bool Succeeded,
    IReadOnlyList<MappedRecord>? Records,
    string? RejectionReason);
