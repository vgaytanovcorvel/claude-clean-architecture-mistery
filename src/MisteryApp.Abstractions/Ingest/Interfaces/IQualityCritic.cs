using MisteryApp.Abstractions.Ingest.Models;

namespace MisteryApp.Abstractions.Ingest.Interfaces;

public interface IQualityCritic
{
    Task<QualityReport> ReviewAsync(MappedRecord record, string sourceFilePath, CancellationToken cancellationToken);
}
