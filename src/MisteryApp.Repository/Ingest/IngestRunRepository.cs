using Microsoft.EntityFrameworkCore;
using MisteryApp.Abstractions.Exceptions;
using MisteryApp.Abstractions.Ingest.Interfaces;
using MisteryApp.Abstractions.Ingest.Models;
using MisteryApp.Common.Enums;
using MisteryApp.Repository.Contexts;
using MisteryApp.Repository.Ingest.Entities;

namespace MisteryApp.Repository.Ingest;

public class IngestRunRepository(
    IDbContextFactory<ApplicationDbContext> contextFactory)
    : RepositoryBase<ApplicationDbContext>(contextFactory), IIngestRunRepository
{
    public virtual async Task<IngestRun> IngestRunAddAsync(IngestRun run, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);
        var entity = MapToEntity(run);
        await dbContext.IngestRuns.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return MapToDomain(entity);
    }

    public virtual async Task<IngestRun> IngestRunUpdateAsync(IngestRun run, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);
        var entity = await dbContext.IngestRuns
            .FirstOrDefaultAsync(r => r.RunId == run.RunId, cancellationToken)
            ?? throw new NotFoundException($"IngestRun not found (RunId: {run.RunId}).");

        entity.Status = run.Status;
        entity.InputPath = run.InputPath;
        entity.CompletedAt = run.CompletedAt;
        entity.TotalFiles = run.TotalFiles;
        entity.ProcessedFiles = run.ProcessedFiles;
        entity.RejectedFiles = run.RejectedFiles;

        await dbContext.SaveChangesAsync(cancellationToken);
        return MapToDomain(entity);
    }

    public virtual async Task<IngestRun> IngestRunSingleByIdAsync(Guid runId, CancellationToken cancellationToken)
    {
        return await IngestRunSingleOrDefaultByIdAsync(runId, cancellationToken)
            ?? throw new NotFoundException($"IngestRun not found (RunId: {runId}).");
    }

    public virtual async Task<IngestRun?> IngestRunSingleOrDefaultByIdAsync(Guid runId, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);
        var entity = await dbContext.IngestRuns
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.RunId == runId, cancellationToken);
        return entity is null ? null : MapToDomain(entity);
    }

    public virtual async Task<IReadOnlyList<IngestRun>> IngestRunGetSummariesAsync(int limit, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);
        return await dbContext.IngestRuns
            .AsNoTracking()
            .OrderByDescending(r => r.StartedAt)
            .Take(limit)
            .Select(r => new IngestRun(
                r.RunId,
                r.Status,
                r.InputPath,
                r.StartedAt,
                r.CompletedAt,
                r.TotalFiles,
                r.ProcessedFiles,
                r.RejectedFiles))
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<IngestRejectedFile> IngestRejectedFileAddAsync(IngestRejectedFile rejectedFile, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);
        var entity = new IngestRejectedFileEntity
        {
            Id = rejectedFile.Id,
            RunId = rejectedFile.RunId,
            FilePath = rejectedFile.FilePath,
            RejectionReason = rejectedFile.RejectionReason,
            RejectedAt = rejectedFile.RejectedAt
        };
        await dbContext.IngestRejectedFiles.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return MapRejectedFileToDomain(entity);
    }

    public virtual async Task<IReadOnlyList<IngestRejectedFile>> IngestRejectedFileGetByRunIdAsync(Guid runId, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);
        return await dbContext.IngestRejectedFiles
            .AsNoTracking()
            .Where(rf => rf.RunId == runId)
            .Select(rf => new IngestRejectedFile(rf.Id, rf.RunId, rf.FilePath, rf.RejectionReason, rf.RejectedAt))
            .ToListAsync(cancellationToken);
    }

    private IngestRun MapToDomain(IngestRunEntity entity) =>
        new(entity.RunId, entity.Status, entity.InputPath, entity.StartedAt,
            entity.CompletedAt, entity.TotalFiles, entity.ProcessedFiles, entity.RejectedFiles);

    private IngestRejectedFile MapRejectedFileToDomain(IngestRejectedFileEntity entity) =>
        new(entity.Id, entity.RunId, entity.FilePath, entity.RejectionReason, entity.RejectedAt);

    private IngestRunEntity MapToEntity(IngestRun run) =>
        new()
        {
            RunId = run.RunId,
            Status = run.Status,
            InputPath = run.InputPath,
            StartedAt = run.StartedAt,
            CompletedAt = run.CompletedAt,
            TotalFiles = run.TotalFiles,
            ProcessedFiles = run.ProcessedFiles,
            RejectedFiles = run.RejectedFiles
        };
}
