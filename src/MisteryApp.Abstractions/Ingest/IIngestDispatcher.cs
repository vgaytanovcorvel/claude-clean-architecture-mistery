using MisteryApp.Common.Ingest;

namespace MisteryApp.Abstractions.Ingest;

public interface IIngestDispatcher
{
    Task<RawFile> ClassifyAsync(string filePath, CancellationToken ct = default);
}
