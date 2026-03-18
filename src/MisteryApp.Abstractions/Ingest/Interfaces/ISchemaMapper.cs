using MisteryApp.Abstractions.Ingest.Models;

namespace MisteryApp.Abstractions.Ingest.Interfaces;

public interface ISchemaMapper
{
    Task<MappedRecord> MapAsync(FileClassification classification, CancellationToken cancellationToken);
}
