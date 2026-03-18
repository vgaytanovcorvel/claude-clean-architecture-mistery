using MisteryApp.Abstractions.Ingest.Models;

namespace MisteryApp.Abstractions.Ingest.Interfaces;

public interface ISchemaMapper
{
    Task<IReadOnlyList<MappedRecord>> MapAsync(FileClassification classification, CancellationToken cancellationToken);
}
