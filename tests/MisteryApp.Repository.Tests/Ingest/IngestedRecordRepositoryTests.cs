using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MisteryApp.Common.Ingest;
using MisteryApp.Repository.Contexts;
using MisteryApp.Repository.Repositories;

namespace MisteryApp.Repository.Tests.Ingest;

[TestClass]
public class IngestedRecordRepositoryTests
{
    private DbContextOptions<ApplicationDbContext> dbOptions = null!;
    private FakeTimeProvider timeProvider = null!;

    [TestInitialize]
    public void Setup()
    {
        dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        timeProvider = new FakeTimeProvider(
            new DateTimeOffset(2025, 6, 15, 10, 0, 0, TimeSpan.Zero));
    }

    [TestMethod]
    public async Task IngestedRecordAddBatchAsync_ShouldInsertAllRecords_WhenRecordsAreValid()
    {
        // Arrange
        var factory = new TestDbContextFactory(dbOptions);
        var repository = new IngestedRecordRepository(factory, timeProvider);
        var records = new List<HealedRecord>
        {
            new(new MappedRecord(new Dictionary<string, object?> { ["id"] = "1", ["source"] = "file1.csv" }, []), ["healed field"]),
            new(new MappedRecord(new Dictionary<string, object?> { ["id"] = "2", ["source"] = "file2.log" }, []), [])
        };

        // Act
        await repository.IngestedRecordAddBatchAsync(records, CancellationToken.None);

        // Assert
        await using var ctx = new ApplicationDbContext(dbOptions);
        var inserted = await ctx.IngestedRecords.ToListAsync();
        inserted.Should().HaveCount(2);
        inserted.Should().AllSatisfy(e => e.RunId.Should().NotBeEmpty());
        inserted.Select(e => e.RunId).Distinct().Should().HaveCount(1);
    }

    [TestMethod]
    public async Task IngestedRecordAddBatchAsync_ShouldSetHealingNotes_WhenRecordWasHealed()
    {
        // Arrange
        var factory = new TestDbContextFactory(dbOptions);
        var repository = new IngestedRecordRepository(factory, timeProvider);
        var records = new List<HealedRecord>
        {
            new(new MappedRecord(new Dictionary<string, object?> { ["value"] = "5" }, []), ["Fixed negative value", "Removed whitespace"])
        };

        // Act
        await repository.IngestedRecordAddBatchAsync(records, CancellationToken.None);

        // Assert
        await using var ctx = new ApplicationDbContext(dbOptions);
        var entity = await ctx.IngestedRecords.SingleAsync();
        entity.HealingNotes.Should().Contain("Fixed negative value");
        entity.HealingNotes.Should().Contain("Removed whitespace");
    }

    [TestMethod]
    public async Task IngestedRecordAddBatchAsync_ShouldSetNullHealingNotes_WhenRecordHadNoIssues()
    {
        // Arrange
        var factory = new TestDbContextFactory(dbOptions);
        var repository = new IngestedRecordRepository(factory, timeProvider);
        var records = new List<HealedRecord>
        {
            new(new MappedRecord(new Dictionary<string, object?> { ["id"] = "1" }, []), [])
        };

        // Act
        await repository.IngestedRecordAddBatchAsync(records, CancellationToken.None);

        // Assert
        await using var ctx = new ApplicationDbContext(dbOptions);
        var entity = await ctx.IngestedRecords.SingleAsync();
        entity.HealingNotes.Should().BeNull();
    }

    [TestMethod]
    public async Task IngestedRecordAddBatchAsync_ShouldDoNothing_WhenRecordsListIsEmpty()
    {
        // Arrange
        var factory = new TestDbContextFactory(dbOptions);
        var repository = new IngestedRecordRepository(factory, timeProvider);

        // Act
        await repository.IngestedRecordAddBatchAsync([], CancellationToken.None);

        // Assert
        await using var ctx = new ApplicationDbContext(dbOptions);
        var count = await ctx.IngestedRecords.CountAsync();
        count.Should().Be(0);
    }

    [TestMethod]
    public async Task IngestedRecordAddBatchAsync_ShouldSetCreatedAtFromTimeProvider_WhenInserting()
    {
        // Arrange
        var factory = new TestDbContextFactory(dbOptions);
        var repository = new IngestedRecordRepository(factory, timeProvider);
        var expectedTime = timeProvider.GetUtcNow();
        var records = new List<HealedRecord>
        {
            new(new MappedRecord(new Dictionary<string, object?> { ["id"] = "1" }, []), [])
        };

        // Act
        await repository.IngestedRecordAddBatchAsync(records, CancellationToken.None);

        // Assert
        await using var ctx = new ApplicationDbContext(dbOptions);
        var entity = await ctx.IngestedRecords.SingleAsync();
        entity.CreatedAt.Should().Be(expectedTime);
    }

    private sealed class TestDbContextFactory(DbContextOptions<ApplicationDbContext> options)
        : IDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext() => new(options);

        public Task<ApplicationDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(new ApplicationDbContext(options));
    }
}
