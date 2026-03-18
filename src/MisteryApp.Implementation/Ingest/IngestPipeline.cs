using MisteryApp.Abstractions.Ingest.Interfaces;
using MisteryApp.Abstractions.Ingest.Models;
using MisteryApp.Common.Enums;

namespace MisteryApp.Implementation.Ingest;

public class IngestPipeline(
    IIngestFileProcessor fileProcessor,
    IIngestRunRepository runRepository,
    TimeProvider timeProvider) : IIngestPipeline
{
    public virtual async Task<IngestRun> RunAsync(
        string inputPath,
        bool dryRun,
        FileFormat hintFormat,
        int parallel,
        CancellationToken cancellationToken)
    {
        if (!File.Exists(inputPath) && !Directory.Exists(inputPath))
            throw new ArgumentException($"Input path does not exist: {inputPath}", nameof(inputPath));

        var fileList = EnumerateFiles(inputPath).ToList();
        var run = new IngestRun(Guid.NewGuid(), IngestRunStatus.Running, inputPath,
            timeProvider.GetUtcNow(), null, fileList.Count, 0, 0);

        if (!dryRun)
            run = await runRepository.IngestRunAddAsync(run, cancellationToken);

        var state = new RunState(fileList.Count);
        await ProcessFilesAsync(fileList, run.RunId, hintFormat, parallel, state, dryRun, cancellationToken);

        var completedRun = run with
        {
            Status = DetermineStatus(state),
            CompletedAt = timeProvider.GetUtcNow(),
            ProcessedFiles = state.ProcessedFiles,
            RejectedFiles = state.RejectedFiles
        };

        if (!dryRun)
            completedRun = await runRepository.IngestRunUpdateAsync(completedRun, cancellationToken);

        return completedRun;
    }

    public virtual async Task<IngestRun> RetryAsync(
        Guid runId,
        bool dryRun,
        int parallel,
        CancellationToken cancellationToken)
    {
        var originalRun = await runRepository.IngestRunSingleByIdAsync(runId, cancellationToken);
        var rejectedFiles = await runRepository.IngestRejectedFileGetByRunIdAsync(runId, cancellationToken);
        var filePaths = rejectedFiles.Select(rf => rf.FilePath).ToList();

        var retryRun = new IngestRun(Guid.NewGuid(), IngestRunStatus.Running, originalRun.InputPath,
            timeProvider.GetUtcNow(), null, filePaths.Count, 0, 0);

        if (!dryRun)
            retryRun = await runRepository.IngestRunAddAsync(retryRun, cancellationToken);

        var state = new RunState(filePaths.Count);
        await ProcessFilesAsync(filePaths, retryRun.RunId, FileFormat.Auto, parallel, state, dryRun, cancellationToken);

        var completedRetryRun = retryRun with
        {
            Status = DetermineStatus(state),
            CompletedAt = timeProvider.GetUtcNow(),
            ProcessedFiles = state.ProcessedFiles,
            RejectedFiles = state.RejectedFiles
        };

        if (!dryRun)
            completedRetryRun = await runRepository.IngestRunUpdateAsync(completedRetryRun, cancellationToken);

        return completedRetryRun;
    }

    private async Task ProcessFilesAsync(
        IReadOnlyList<string> files,
        Guid runId,
        FileFormat hintFormat,
        int parallel,
        RunState state,
        bool dryRun,
        CancellationToken cancellationToken)
    {
        using var semaphore = new SemaphoreSlim(parallel, parallel);
        var tasks = files.Select(file =>
            ProcessSingleFileAsync(file, runId, hintFormat, semaphore, state, dryRun, cancellationToken));
        await Task.WhenAll(tasks);
    }

    private async Task ProcessSingleFileAsync(
        string filePath,
        Guid runId,
        FileFormat hintFormat,
        SemaphoreSlim semaphore,
        RunState state,
        bool dryRun,
        CancellationToken cancellationToken)
    {
        await semaphore.WaitAsync(cancellationToken);
        try
        {
            var result = await fileProcessor.ProcessFileAsync(filePath, hintFormat, cancellationToken);
            state.IncrementProcessed();

            if (result.Succeeded)
            {
                if (!dryRun && result.Records is { Count: > 0 })
                    await runRepository.MappedRecordAddRangeAsync(runId, filePath, result.Records, cancellationToken);
            }
            else
            {
                state.IncrementRejected();
                if (!dryRun)
                    await SaveRejectedFileAsync(runId, filePath, result.RejectionReason, cancellationToken);
            }
        }
        finally
        {
            semaphore.Release();
        }
    }

    private async Task SaveRejectedFileAsync(
        Guid runId,
        string filePath,
        string? reason,
        CancellationToken cancellationToken)
    {
        var rejected = new IngestRejectedFile(
            Guid.NewGuid(), runId, filePath, reason ?? "Unknown", timeProvider.GetUtcNow());
        await runRepository.IngestRejectedFileAddAsync(rejected, cancellationToken);
    }

    public virtual async Task<IReadOnlyList<IngestRun>> GetRunSummariesAsync(int limit, CancellationToken cancellationToken)
        => await runRepository.IngestRunGetSummariesAsync(limit, cancellationToken);

    private static IEnumerable<string> EnumerateFiles(string inputPath)
    {
        if (File.Exists(inputPath)) return [inputPath];
        if (Directory.Exists(inputPath))
            return Directory.EnumerateFiles(inputPath, "*", SearchOption.AllDirectories);
        return [];
    }

    private static IngestRunStatus DetermineStatus(RunState state)
    {
        if (state.ProcessedFiles == 0) return IngestRunStatus.Failed;
        if (state.RejectedFiles == 0) return IngestRunStatus.Completed;
        if (state.RejectedFiles < state.ProcessedFiles) return IngestRunStatus.PartialSuccess;
        return IngestRunStatus.Failed;
    }

    private sealed class RunState(int totalFiles)
    {
        public readonly int TotalFiles = totalFiles;
        private int _processedFiles;
        private int _rejectedFiles;

        public int ProcessedFiles => _processedFiles;
        public int RejectedFiles => _rejectedFiles;

        public void IncrementProcessed() => Interlocked.Increment(ref _processedFiles);
        public void IncrementRejected() => Interlocked.Increment(ref _rejectedFiles);
    }
}
