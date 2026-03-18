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

    public virtual async Task<IReadOnlyList<IngestRunSummary>> IngestRunGetSummariesAsync(int limit, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);
        return await dbContext.IngestRuns
            .AsNoTracking()
            .OrderByDescending(r => r.StartedAt)
            .Take(limit)
            .Select(r => new IngestRunSummary(
                r.RunId,
                r.Status,
                r.InputPath,
                r.StartedAt,
                r.CompletedAt,
                r.Files.Count,
                r.Files.Count(f => f.RejectedFile == null),
                r.Files.Count(f => f.RejectedFile != null)))
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<IngestRejectedFile> IngestRejectedFileAddAsync(
        Guid runId,
        string filePath,
        string rejectionReason,
        CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);

        var fileEntity = new IngestFileEntity
        {
            Id = Guid.NewGuid(),
            RunId = runId,
            FilePath = filePath,
            ProcessedAt = DateTimeOffset.UtcNow
        };
        await dbContext.IngestFiles.AddAsync(fileEntity, cancellationToken);

        var rejectedEntity = new IngestRejectedFileEntity
        {
            Id = Guid.NewGuid(),
            FileId = fileEntity.Id,
            RejectionReason = rejectionReason
        };
        await dbContext.IngestRejectedFiles.AddAsync(rejectedEntity, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new IngestRejectedFile(rejectedEntity.Id, fileEntity.Id, rejectionReason);
    }

    public virtual async Task<IReadOnlyList<string>> IngestRejectedFilePathsGetByRunIdAsync(Guid runId, CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);
        return await dbContext.IngestFiles
            .AsNoTracking()
            .Where(f => f.RunId == runId && f.RejectedFile != null)
            .Select(f => f.FilePath)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<MappedRecord>> MappedRecordAddRangeAsync(
        Guid runId,
        string filePath,
        IReadOnlyList<MappedRecord> records,
        CancellationToken cancellationToken)
    {
        await using var dbContext = await CreateContextAsync(cancellationToken);

        var fileEntity = new IngestFileEntity
        {
            Id = Guid.NewGuid(),
            RunId = runId,
            FilePath = filePath,
            ProcessedAt = DateTimeOffset.UtcNow
        };
        await dbContext.IngestFiles.AddAsync(fileEntity, cancellationToken);

        var entities = records.Select(record => new MappedRecordEntity
        {
            Id = Guid.NewGuid(),
            FileId = fileEntity.Id,
            ExternalId = record.Id,
            Timestamp = record.Timestamp,
            Value = record.Value,
            Description = record.Description,
            Source = record.Source,
            Category = record.Category,
            IngestedAt = DateTimeOffset.UtcNow
        }).ToList();
        dbContext.MappedRecords.AddRange(entities);

        await dbContext.SaveChangesAsync(cancellationToken);
        return entities.Select(MapMappedRecordToDomain).ToList();
    }

    private static IngestRun MapToDomain(IngestRunEntity entity) =>
        new(entity.RunId, entity.Status, entity.InputPath, entity.StartedAt, entity.CompletedAt);

    private static IngestRunEntity MapToEntity(IngestRun run) =>
        new()
        {
            RunId = run.RunId,
            Status = run.Status,
            InputPath = run.InputPath,
            StartedAt = run.StartedAt,
            CompletedAt = run.CompletedAt
        };

    private static MappedRecord MapMappedRecordToDomain(MappedRecordEntity entity) =>
        new(entity.ExternalId, entity.Timestamp, entity.Value, entity.Description, entity.Source, entity.Category);
}
