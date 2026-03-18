using MisteryApp.Abstractions.Ingest.Models;

namespace MisteryApp.Abstractions.Ingest.Interfaces;

public interface IIngestRunRepository
{
    Task<IngestRun> IngestRunAddAsync(IngestRun run, CancellationToken cancellationToken);
    Task<IngestRun> IngestRunUpdateAsync(IngestRun run, CancellationToken cancellationToken);
    Task<IngestRun> IngestRunSingleByIdAsync(Guid runId, CancellationToken cancellationToken);
    Task<IngestRun?> IngestRunSingleOrDefaultByIdAsync(Guid runId, CancellationToken cancellationToken);
    Task<IReadOnlyList<IngestRunSummary>> IngestRunGetSummariesAsync(int limit, CancellationToken cancellationToken);
    Task<IngestRejectedFile> IngestRejectedFileAddAsync(Guid runId, string filePath, string rejectionReason, CancellationToken cancellationToken);
    Task<IReadOnlyList<string>> IngestRejectedFilePathsGetByRunIdAsync(Guid runId, CancellationToken cancellationToken);
    Task<IReadOnlyList<MappedRecord>> MappedRecordAddRangeAsync(Guid runId, string filePath, IReadOnlyList<MappedRecord> records, CancellationToken cancellationToken);
}
