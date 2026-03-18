using MisteryApp.Common.Ingest;

namespace MisteryApp.Abstractions.Ingest;

public interface ISchemaMapper
{
    Task<MappedRecord> MapAsync(RawFile raw, CancellationToken ct = default);
}
