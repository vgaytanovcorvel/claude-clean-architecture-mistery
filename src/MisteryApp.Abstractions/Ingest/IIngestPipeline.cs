using MisteryApp.Common.Ingest;

namespace MisteryApp.Abstractions.Ingest;

public interface IIngestPipeline
{
    Task<IngestRunSummary> RunAsync(
        string path,
        bool dryRun,
        IngestFormat formatHint,
        CancellationToken ct = default);
}
