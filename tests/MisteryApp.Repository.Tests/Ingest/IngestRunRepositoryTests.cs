using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using MisteryApp.Abstractions.Exceptions;
using MisteryApp.Abstractions.Ingest.Models;
using MisteryApp.Common.Enums;
using MisteryApp.Repository.Contexts;
using MisteryApp.Repository.Ingest;
using MisteryApp.Repository.Ingest.Entities;

namespace MisteryApp.Repository.Tests.Ingest;

[TestClass]
public class IngestRunRepositoryTests
{
    private Mock<IDbContextFactory<ApplicationDbContext>> contextFactoryMock = new(MockBehavior.Strict);
    private Mock<IngestRunRepository> repositoryMock = null!;
    private CancellationToken cancellationToken = CancellationToken.None;

    private static readonly DateTimeOffset FixedTime = new(2024, 1, 15, 10, 0, 0, TimeSpan.Zero);

    [TestInitialize]
    public void Setup()
    {
        contextFactoryMock = new Mock<IDbContextFactory<ApplicationDbContext>>(MockBehavior.Strict);
        repositoryMock = new Mock<IngestRunRepository>(
            () => new IngestRunRepository(contextFactoryMock.Object),
            MockBehavior.Strict);

        repositoryMock
            .Protected()
            .Setup<Task<ApplicationDbContext>>(
                "CreateContextAsync",
                ItExpr.IsAny<CancellationToken>())
            .CallBase();
    }

    [TestMethod]
    public async Task IngestRunAddAsync_ShouldReturnPersistedRun_WhenRunIsValid()
    {
        // Arrange
        var run = BuildRun();
        var ctx = BuildInMemoryContext();

        contextFactoryMock
            .Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ctx)
            .Verifiable(Times.Once());

        repositoryMock
            .Setup(r => r.IngestRunAddAsync(run, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        var result = await repositoryMock.Object.IngestRunAddAsync(run, cancellationToken);

        // Assert
        result.RunId.Should().Be(run.RunId);
        result.Status.Should().Be(IngestRunStatus.Running);
        result.InputPath.Should().Be(run.InputPath);

        repositoryMock.VerifyAll();
        contextFactoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task IngestRunUpdateAsync_ShouldReturnUpdatedRun_WhenRunExists()
    {
        // Arrange
        var run = BuildRun();
        var ctx = BuildInMemoryContext();

        // Seed entity so update can find it
        ctx.IngestRuns.Add(new IngestRunEntity
        {
            RunId = run.RunId,
            Status = run.Status,
            InputPath = run.InputPath,
            StartedAt = run.StartedAt,
            CompletedAt = run.CompletedAt,
            TotalFiles = run.TotalFiles,
            ProcessedFiles = run.ProcessedFiles,
            RejectedFiles = run.RejectedFiles
        });
        await ctx.SaveChangesAsync(cancellationToken);

        var updated = run with { Status = IngestRunStatus.Completed, ProcessedFiles = 5, RejectedFiles = 1, CompletedAt = FixedTime };

        contextFactoryMock
            .Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ctx)
            .Verifiable(Times.Once());

        repositoryMock
            .Setup(r => r.IngestRunUpdateAsync(updated, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        var result = await repositoryMock.Object.IngestRunUpdateAsync(updated, cancellationToken);

        // Assert
        result.Status.Should().Be(IngestRunStatus.Completed);
        result.ProcessedFiles.Should().Be(5);
        result.RejectedFiles.Should().Be(1);

        repositoryMock.VerifyAll();
        contextFactoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task IngestRunSingleByIdAsync_ShouldThrowNotFoundException_WhenNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var ctx = BuildInMemoryContext();

        contextFactoryMock
            .Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ctx)
            .Verifiable(Times.Once());

        repositoryMock
            .Setup(r => r.IngestRunSingleByIdAsync(nonExistentId, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        repositoryMock
            .Setup(r => r.IngestRunSingleOrDefaultByIdAsync(nonExistentId, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<NotFoundException>(
            () => repositoryMock.Object.IngestRunSingleByIdAsync(nonExistentId, cancellationToken));

        exception.Message.Should().Contain(nonExistentId.ToString());

        repositoryMock.VerifyAll();
        contextFactoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task IngestRunSingleOrDefaultByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var ctx = BuildInMemoryContext();

        contextFactoryMock
            .Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ctx)
            .Verifiable(Times.Once());

        repositoryMock
            .Setup(r => r.IngestRunSingleOrDefaultByIdAsync(nonExistentId, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        var result = await repositoryMock.Object.IngestRunSingleOrDefaultByIdAsync(nonExistentId, cancellationToken);

        // Assert
        result.Should().BeNull();

        repositoryMock.VerifyAll();
        contextFactoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task IngestRunGetSummariesAsync_ShouldRespectLimit_WhenMultipleRunsExist()
    {
        // Arrange
        var ctx = BuildInMemoryContext();

        for (var i = 0; i < 5; i++)
        {
            ctx.IngestRuns.Add(new IngestRunEntity
            {
                RunId = Guid.NewGuid(),
                Status = IngestRunStatus.Completed,
                InputPath = "/data/input",
                StartedAt = FixedTime.AddHours(i),
                TotalFiles = 10,
                ProcessedFiles = 10,
                RejectedFiles = 0
            });
        }
        await ctx.SaveChangesAsync(cancellationToken);

        contextFactoryMock
            .Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ctx)
            .Verifiable(Times.Once());

        repositoryMock
            .Setup(r => r.IngestRunGetSummariesAsync(3, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        var summaries = await repositoryMock.Object.IngestRunGetSummariesAsync(3, cancellationToken);

        // Assert
        summaries.Should().HaveCount(3);
        summaries.Select(s => s.StartedAt).Should().BeInDescendingOrder();

        repositoryMock.VerifyAll();
        contextFactoryMock.VerifyAll();
    }

    [TestMethod]
    public async Task IngestRejectedFileAddAsync_ShouldReturnPersistedFile_WhenCalled()
    {
        // Arrange
        var runId = Guid.NewGuid();
        var ctx = BuildInMemoryContext();

        // Seed parent run to satisfy FK
        ctx.IngestRuns.Add(new IngestRunEntity
        {
            RunId = runId,
            Status = IngestRunStatus.Running,
            InputPath = "/data/input",
            StartedAt = FixedTime,
            TotalFiles = 10,
            ProcessedFiles = 0,
            RejectedFiles = 0
        });
        await ctx.SaveChangesAsync(cancellationToken);

        var rejectedFile = new IngestRejectedFile(Guid.NewGuid(), runId, "/data/bad.csv", "Hallucination", FixedTime);

        contextFactoryMock
            .Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ctx)
            .Verifiable(Times.Once());

        repositoryMock
            .Setup(r => r.IngestRejectedFileAddAsync(rejectedFile, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        var result = await repositoryMock.Object.IngestRejectedFileAddAsync(rejectedFile, cancellationToken);

        // Assert
        result.Id.Should().Be(rejectedFile.Id);
        result.FilePath.Should().Be("/data/bad.csv");
        result.RejectionReason.Should().Be("Hallucination");

        repositoryMock.VerifyAll();
        contextFactoryMock.VerifyAll();
    }

    private static IngestRun BuildRun() =>
        new(Guid.NewGuid(), IngestRunStatus.Running, "/data/input", FixedTime, null, 10, 0, 0);

    private static ApplicationDbContext BuildInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }
}
