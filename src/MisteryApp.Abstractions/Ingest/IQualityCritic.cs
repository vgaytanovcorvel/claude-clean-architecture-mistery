using MisteryApp.Common.Ingest;

namespace MisteryApp.Abstractions.Ingest;

public interface IQualityCritic
{
    Task<IReadOnlyList<QualityIssue>> CritiqueAsync(MappedRecord record, CancellationToken ct = default);
}
