using MisteryApp.Common.Ingest;

namespace MisteryApp.Abstractions.Ingest;

public interface IIngestedRecordRepository
{
    Task IngestedRecordAddBatchAsync(IReadOnlyList<HealedRecord> records, CancellationToken ct = default);
}
