using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MisteryApp.Abstractions.Exceptions;
using MisteryApp.Abstractions.Ingest.Interfaces;
using MisteryApp.Abstractions.Ingest.Models;
using MisteryApp.Common.Enums;
using MisteryApp.Implementation.Ingest;

namespace MisteryApp.Implementation.Tests.Ingest;

[TestClass]
public class IngestPipelineTests
{
    private Mock<IIngestFileProcessor> fileProcessorMock = new(MockBehavior.Strict);
    private Mock<IIngestRunRepository> runRepositoryMock = new(MockBehavior.Strict);
    private FakeTimeProvider timeProvider = null!;
    private Mock<IngestPipeline> pipelineMock = null!;
    private CancellationToken cancellationToken = CancellationToken.None;

    private static readonly DateTimeOffset FixedTime = new(2024, 1, 15, 10, 0, 0, TimeSpan.Zero);

    [TestInitialize]
    public void Setup()
    {
        timeProvider = new FakeTimeProvider(FixedTime);
        pipelineMock = new Mock<IngestPipeline>(
            () => new IngestPipeline(fileProcessorMock.Object, runRepositoryMock.Object, timeProvider),
            MockBehavior.Strict);
    }

    [TestMethod]
    public async Task RunAsync_ShouldReturnCompletedRun_WhenSingleFileSucceeds()
    {
        // Arrange
        var tmpFile = CreateTempFile();
        var successResult = new FileProcessResult(tmpFile, true, new MappedRecord("1", null, 1m, null, null, null), null);
        var savedRun = BuildRun(tmpFile, IngestRunStatus.Running, 1, 0, 0);
        var updatedRun = savedRun with { Status = IngestRunStatus.Completed, ProcessedFiles = 1, CompletedAt = FixedTime };

        pipelineMock
            .Setup(p => p.RunAsync(tmpFile, false, FileFormat.Auto, 4, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        runRepositoryMock
            .Setup(r => r.IngestRunAddAsync(It.Is<IngestRun>(run => run.InputPath == tmpFile && run.TotalFiles == 1), cancellationToken))
            .ReturnsAsync(savedRun)
            .Verifiable(Times.Once());

        fileProcessorMock
            .Setup(p => p.ProcessFileAsync(tmpFile, FileFormat.Auto, cancellationToken))
            .ReturnsAsync(successResult)
            .Verifiable(Times.Once());

        runRepositoryMock
            .Setup(r => r.IngestRunUpdateAsync(It.Is<IngestRun>(run => run.Status == IngestRunStatus.Completed), cancellationToken))
            .ReturnsAsync(updatedRun)
            .Verifiable(Times.Once());

        try
        {
            // Act
            var result = await pipelineMock.Object.RunAsync(tmpFile, false, FileFormat.Auto, 4, cancellationToken);

            // Assert
            result.Status.Should().Be(IngestRunStatus.Completed);
            result.RejectedFiles.Should().Be(0);

            pipelineMock.VerifyAll();
            fileProcessorMock.VerifyAll();
            runRepositoryMock.VerifyAll();
        }
        finally
        {
            File.Delete(tmpFile);
        }
    }

    [TestMethod]
    public async Task RunAsync_ShouldReturnPartialSuccess_WhenSomeFilesRejected()
    {
        // Arrange
        var tmpDir = CreateTempDirectory();
        var file1 = Path.Combine(tmpDir, "a.log");
        var file2 = Path.Combine(tmpDir, "b.log");
        File.WriteAllText(file1, "ok");
        File.WriteAllText(file2, "bad");

        var successResult = new FileProcessResult(file1, true, new MappedRecord("1", null, 1m, null, null, null), null);
        var failResult = new FileProcessResult(file2, false, null, "Hallucination detected");
        var savedRun = BuildRun(tmpDir, IngestRunStatus.Running, 2, 0, 0);
        var updatedRun = savedRun with { Status = IngestRunStatus.PartialSuccess, ProcessedFiles = 2, RejectedFiles = 1, CompletedAt = FixedTime };

        pipelineMock
            .Setup(p => p.RunAsync(tmpDir, false, FileFormat.Auto, 4, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        runRepositoryMock
            .Setup(r => r.IngestRunAddAsync(It.IsAny<IngestRun>(), cancellationToken))
            .ReturnsAsync(savedRun)
            .Verifiable(Times.Once());

        fileProcessorMock
            .Setup(p => p.ProcessFileAsync(It.Is<string>(f => f == file1), FileFormat.Auto, cancellationToken))
            .ReturnsAsync(successResult)
            .Verifiable(Times.Once());

        fileProcessorMock
            .Setup(p => p.ProcessFileAsync(It.Is<string>(f => f == file2), FileFormat.Auto, cancellationToken))
            .ReturnsAsync(failResult)
            .Verifiable(Times.Once());

        runRepositoryMock
            .Setup(r => r.IngestRejectedFileAddAsync(It.IsAny<IngestRejectedFile>(), cancellationToken))
            .Returns((IngestRejectedFile rf, CancellationToken _) => Task.FromResult(rf))
            .Verifiable(Times.Once());

        runRepositoryMock
            .Setup(r => r.IngestRunUpdateAsync(It.Is<IngestRun>(run => run.Status == IngestRunStatus.PartialSuccess), cancellationToken))
            .ReturnsAsync(updatedRun)
            .Verifiable(Times.Once());

        try
        {
            // Act
            var result = await pipelineMock.Object.RunAsync(tmpDir, false, FileFormat.Auto, 4, cancellationToken);

            // Assert
            result.Status.Should().Be(IngestRunStatus.PartialSuccess);

            pipelineMock.VerifyAll();
            fileProcessorMock.VerifyAll();
            runRepositoryMock.VerifyAll();
        }
        finally
        {
            Directory.Delete(tmpDir, true);
        }
    }

    [TestMethod]
    public async Task RunAsync_ShouldNotPersist_WhenDryRun()
    {
        // Arrange
        var tmpFile = CreateTempFile();
        var successResult = new FileProcessResult(tmpFile, true, new MappedRecord("1", null, 1m, null, null, null), null);

        pipelineMock
            .Setup(p => p.RunAsync(tmpFile, true, FileFormat.Auto, 4, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        fileProcessorMock
            .Setup(p => p.ProcessFileAsync(tmpFile, FileFormat.Auto, cancellationToken))
            .ReturnsAsync(successResult)
            .Verifiable(Times.Once());

        try
        {
            // Act
            var result = await pipelineMock.Object.RunAsync(tmpFile, true, FileFormat.Auto, 4, cancellationToken);

            // Assert
            result.Status.Should().Be(IngestRunStatus.Completed);

            // No DB calls expected
            pipelineMock.VerifyAll();
            fileProcessorMock.VerifyAll();
            runRepositoryMock.VerifyAll();
        }
        finally
        {
            File.Delete(tmpFile);
        }
    }

    [TestMethod]
    public async Task RetryAsync_ShouldProcessRejectedFiles_WhenRunExists()
    {
        // Arrange
        var runId = Guid.NewGuid();
        var tmpFile = CreateTempFile();
        var originalRun = BuildRun(tmpFile, IngestRunStatus.PartialSuccess, 2, 2, 1);
        var rejectedFiles = new List<IngestRejectedFile>
        {
            new(Guid.NewGuid(), runId, tmpFile, "Some issue", FixedTime)
        };
        var successResult = new FileProcessResult(tmpFile, true, new MappedRecord("1", null, 1m, null, null, null), null);
        var savedRetryRun = BuildRun(tmpFile, IngestRunStatus.Running, 1, 0, 0);
        var updatedRetryRun = savedRetryRun with { Status = IngestRunStatus.Completed, ProcessedFiles = 1, CompletedAt = FixedTime };

        pipelineMock
            .Setup(p => p.RetryAsync(runId, false, 4, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        runRepositoryMock
            .Setup(r => r.IngestRunSingleByIdAsync(runId, cancellationToken))
            .ReturnsAsync(originalRun)
            .Verifiable(Times.Once());

        runRepositoryMock
            .Setup(r => r.IngestRejectedFileGetByRunIdAsync(runId, cancellationToken))
            .ReturnsAsync(rejectedFiles)
            .Verifiable(Times.Once());

        runRepositoryMock
            .Setup(r => r.IngestRunAddAsync(It.IsAny<IngestRun>(), cancellationToken))
            .ReturnsAsync(savedRetryRun)
            .Verifiable(Times.Once());

        fileProcessorMock
            .Setup(p => p.ProcessFileAsync(tmpFile, FileFormat.Auto, cancellationToken))
            .ReturnsAsync(successResult)
            .Verifiable(Times.Once());

        runRepositoryMock
            .Setup(r => r.IngestRunUpdateAsync(It.IsAny<IngestRun>(), cancellationToken))
            .ReturnsAsync(updatedRetryRun)
            .Verifiable(Times.Once());

        try
        {
            // Act
            var result = await pipelineMock.Object.RetryAsync(runId, false, 4, cancellationToken);

            // Assert
            result.Status.Should().Be(IngestRunStatus.Completed);

            pipelineMock.VerifyAll();
            fileProcessorMock.VerifyAll();
            runRepositoryMock.VerifyAll();
        }
        finally
        {
            File.Delete(tmpFile);
        }
    }

    [TestMethod]
    public async Task RetryAsync_ShouldThrowNotFoundException_WhenRunNotFound()
    {
        // Arrange
        var runId = Guid.NewGuid();

        pipelineMock
            .Setup(p => p.RetryAsync(runId, false, 4, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        runRepositoryMock
            .Setup(r => r.IngestRunSingleByIdAsync(runId, cancellationToken))
            .ThrowsAsync(new NotFoundException($"IngestRun not found (RunId: {runId})."))
            .Verifiable(Times.Once());

        // Act & Assert
        await Assert.ThrowsExceptionAsync<NotFoundException>(
            () => pipelineMock.Object.RetryAsync(runId, false, 4, cancellationToken));

        pipelineMock.VerifyAll();
        runRepositoryMock.VerifyAll();
        fileProcessorMock.VerifyAll();
    }

    [TestMethod]
    public async Task RunAsync_ShouldReturnFailed_WhenNoFilesFound()
    {
        // Arrange
        var emptyDir = CreateTempDirectory();
        var savedRun = BuildRun(emptyDir, IngestRunStatus.Running, 0, 0, 0);
        var updatedRun = savedRun with { Status = IngestRunStatus.Failed, CompletedAt = FixedTime };

        pipelineMock
            .Setup(p => p.RunAsync(emptyDir, false, FileFormat.Auto, 4, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        runRepositoryMock
            .Setup(r => r.IngestRunAddAsync(It.IsAny<IngestRun>(), cancellationToken))
            .ReturnsAsync(savedRun)
            .Verifiable(Times.Once());

        runRepositoryMock
            .Setup(r => r.IngestRunUpdateAsync(It.Is<IngestRun>(r => r.Status == IngestRunStatus.Failed), cancellationToken))
            .ReturnsAsync(updatedRun)
            .Verifiable(Times.Once());

        try
        {
            // Act
            var result = await pipelineMock.Object.RunAsync(emptyDir, false, FileFormat.Auto, 4, cancellationToken);

            // Assert
            result.Status.Should().Be(IngestRunStatus.Failed);

            pipelineMock.VerifyAll();
            fileProcessorMock.VerifyAll();
            runRepositoryMock.VerifyAll();
        }
        finally
        {
            Directory.Delete(emptyDir, true);
        }
    }

    [TestMethod]
    public async Task GetRunSummariesAsync_ShouldDelegateToRepository_WhenCalled()
    {
        // Arrange
        var runs = new List<IngestRun>
        {
            BuildRun("/data", IngestRunStatus.Completed, 5, 5, 0)
        };

        pipelineMock
            .Setup(p => p.GetRunSummariesAsync(10, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        runRepositoryMock
            .Setup(r => r.IngestRunGetSummariesAsync(10, cancellationToken))
            .ReturnsAsync(runs)
            .Verifiable(Times.Once());

        // Act
        var result = await pipelineMock.Object.GetRunSummariesAsync(10, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(runs);

        pipelineMock.VerifyAll();
        runRepositoryMock.VerifyAll();
        fileProcessorMock.VerifyAll();
    }

    private static IngestRun BuildRun(string path, IngestRunStatus status, int total, int processed, int rejected) =>
        new(Guid.NewGuid(), status, path, FixedTime, null, total, processed, rejected);

    private static string CreateTempFile()
    {
        var path = Path.GetTempFileName();
        File.WriteAllText(path, "test content");
        return path;
    }

    private static string CreateTempDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(path);
        return path;
    }
}
