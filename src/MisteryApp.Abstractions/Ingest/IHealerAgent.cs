using MisteryApp.Common.Ingest;

namespace MisteryApp.Abstractions.Ingest;

public interface IHealerAgent
{
    Task<HealedRecord?> HealAsync(
        MappedRecord record,
        IReadOnlyList<QualityIssue> issues,
        CancellationToken ct = default);
}
