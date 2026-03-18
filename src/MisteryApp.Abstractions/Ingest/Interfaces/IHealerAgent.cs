using MisteryApp.Abstractions.Ingest.Models;
using MisteryApp.Abstractions.Models;

namespace MisteryApp.Abstractions.Ingest.Interfaces;

public interface IHealerAgent
{
    Task<Result<MappedRecord>> HealAsync(QualityReport report, CancellationToken cancellationToken);
}
