using MisteryApp.Abstractions.Ingest;
using MisteryApp.Common.Ingest;

namespace MisteryApp.Implementation.Ingest;

public class IngestPipeline(
    IIngestDispatcher dispatcher,
    ISchemaMapper schemaMapper,
    IQualityCritic qualityCritic,
    IHealerAgent healerAgent,
    IIngestedRecordRepository repository) : IIngestPipeline
{
    public virtual async Task<IngestRunSummary> RunAsync(
        string path,
        bool dryRun,
        IngestFormat formatHint,
        CancellationToken ct = default)
    {
        var files = EnumerateFiles(path).ToList();
        var state = new RunState();

        foreach (var file in files)
            await ProcessFileAsync(file, formatHint, state, ct);

        if (!dryRun && state.HealedRecords.Count > 0)
            await repository.IngestedRecordAddBatchAsync(state.HealedRecords, ct);

        return new IngestRunSummary(
            files.Count,
            dryRun ? 0 : state.HealedRecords.Count,
            state.RecordsHealed,
            state.RecordsRejected,
            state.Errors);
    }

    internal virtual async Task ProcessFileAsync(
        string filePath,
        IngestFormat formatHint,
        RunState state,
        CancellationToken ct)
    {
        try
        {
            var rawFile = await dispatcher.ClassifyAsync(filePath, ct);
            var effectiveFile = ApplyFormatHint(rawFile, formatHint);
            var mapped = await schemaMapper.MapAsync(effectiveFile, ct);
            await EvaluateRecordAsync(mapped, filePath, state, ct);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            state.AddError($"Failed to process {filePath}: {ex.Message}");
        }
    }

    internal virtual async Task EvaluateRecordAsync(
        MappedRecord mapped,
        string filePath,
        RunState state,
        CancellationToken ct)
    {
        var issues = await qualityCritic.CritiqueAsync(mapped, ct);
        if (issues.Count == 0)
        {
            state.HealedRecords.Add(new HealedRecord(mapped, []));
            return;
        }

        var healed = await healerAgent.HealAsync(mapped, issues, ct);
        if (healed is null)
        {
            state.RecordsRejected++;
            state.AddError($"Could not heal record from {filePath}.");
            return;
        }

        state.RecordsHealed++;
        state.HealedRecords.Add(healed);
    }

    internal virtual IEnumerable<string> EnumerateFiles(string path)
    {
        if (File.Exists(path))
            return [path];
        if (Directory.Exists(path))
            return Directory.EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly);
        return [];
    }

    private static RawFile ApplyFormatHint(RawFile raw, IngestFormat hint)
        => hint == IngestFormat.Auto ? raw : raw with { DetectedFormat = hint };

    internal sealed class RunState
    {
        public List<HealedRecord> HealedRecords { get; } = [];
        public int RecordsHealed { get; set; }
        public int RecordsRejected { get; set; }

        private readonly List<string> _errors = [];
        public IReadOnlyList<string> Errors => _errors;
        public void AddError(string error) => _errors.Add(error);
    }
}
