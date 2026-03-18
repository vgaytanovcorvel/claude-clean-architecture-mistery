using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MisteryApp.Abstractions.Ingest;
using MisteryApp.Common.Ingest;
using MisteryApp.Repository.Contexts;
using MisteryApp.Repository.Entities;

namespace MisteryApp.Repository.Repositories;

public class IngestedRecordRepository(
    IDbContextFactory<ApplicationDbContext> contextFactory,
    TimeProvider timeProvider)
    : RepositoryBase<ApplicationDbContext>(contextFactory), IIngestedRecordRepository
{
    public virtual async Task IngestedRecordAddBatchAsync(
        IReadOnlyList<HealedRecord> records,
        CancellationToken ct = default)
    {
        if (records.Count == 0)
            return;

        await using var ctx = await CreateContextAsync(ct);
        var runId = Guid.NewGuid();
        var entities = records.Select(r => MapToEntity(r, runId)).ToList();
        await ctx.IngestedRecords.AddRangeAsync(entities, ct);
        await ctx.SaveChangesAsync(ct);
    }

    private IngestedRecordEntity MapToEntity(HealedRecord record, Guid runId) => new()
    {
        Id = Guid.NewGuid(),
        SourceFile = GetFieldString(record.Record.Fields, "source"),
        Format = GetFieldString(record.Record.Fields, "format"),
        FieldsJson = JsonSerializer.Serialize(record.Record.Fields),
        HealingNotes = record.HealingNotes.Count > 0
            ? string.Join("; ", record.HealingNotes)
            : null,
        CreatedAt = timeProvider.GetUtcNow(),
        RunId = runId
    };

    private static string GetFieldString(IReadOnlyDictionary<string, object?> fields, string key)
        => fields.TryGetValue(key, out var value) ? value?.ToString() ?? string.Empty : string.Empty;
}
