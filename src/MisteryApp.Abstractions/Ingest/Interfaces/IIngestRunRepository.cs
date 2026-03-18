using MisteryApp.Abstractions.Ingest.Models;

namespace MisteryApp.Abstractions.Ingest.Interfaces;

public interface IIngestRunRepository
{
    Task<IngestRun> IngestRunAddAsync(IngestRun run, CancellationToken cancellationToken);
    Task<IngestRun> IngestRunUpdateAsync(IngestRun run, CancellationToken cancellationToken);
    Task<IngestRun> IngestRunSingleByIdAsync(Guid runId, CancellationToken cancellationToken);
    Task<IngestRun?> IngestRunSingleOrDefaultByIdAsync(Guid runId, CancellationToken cancellationToken);
    Task<IReadOnlyList<IngestRun>> IngestRunGetSummariesAsync(int limit, CancellationToken cancellationToken);
    Task<IngestRejectedFile> IngestRejectedFileAddAsync(IngestRejectedFile rejectedFile, CancellationToken cancellationToken);
    Task<IReadOnlyList<IngestRejectedFile>> IngestRejectedFileGetByRunIdAsync(Guid runId, CancellationToken cancellationToken);
}
