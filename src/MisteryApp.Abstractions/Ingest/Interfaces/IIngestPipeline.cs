using MisteryApp.Abstractions.Ingest.Models;
using MisteryApp.Common.Enums;

namespace MisteryApp.Abstractions.Ingest.Interfaces;

public interface IIngestPipeline
{
    Task<IngestRun> RunAsync(
        string inputPath,
        bool dryRun,
        FileFormat hintFormat,
        int parallel,
        CancellationToken cancellationToken);

    Task<IngestRun> RetryAsync(
        Guid runId,
        bool dryRun,
        int parallel,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<IngestRunSummary>> GetRunSummariesAsync(int limit, CancellationToken cancellationToken);
}
