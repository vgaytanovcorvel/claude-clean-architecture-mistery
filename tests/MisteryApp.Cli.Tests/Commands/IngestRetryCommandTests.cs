using System.CommandLine;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MisteryApp.Abstractions.Exceptions;
using MisteryApp.Abstractions.Ingest.Interfaces;
using MisteryApp.Abstractions.Ingest.Models;
using MisteryApp.Cli.Commands;
using MisteryApp.Common.Enums;

namespace MisteryApp.Cli.Tests.Commands;

[TestClass]
public class IngestRetryCommandTests
{
    private Mock<IIngestPipeline> pipelineMock = new(MockBehavior.Strict);
    private IHost host = null!;

    private static readonly DateTimeOffset FixedTime = new(2024, 1, 15, 10, 0, 0, TimeSpan.Zero);

    [TestInitialize]
    public void Setup()
    {
        host = Host.CreateDefaultBuilder()
            .ConfigureServices(services => services.AddSingleton(pipelineMock.Object))
            .Build();
    }

    [TestCleanup]
    public void Cleanup() => host.Dispose();

    [TestMethod]
    public async Task IngestRetry_ShouldExitZero_WhenRetrySucceeds()
    {
        // Arrange
        var runId = Guid.NewGuid();
        var completedRun = new IngestRun(Guid.NewGuid(), IngestRunStatus.Completed, "/data", FixedTime, FixedTime);

        pipelineMock
            .Setup(p => p.RetryAsync(runId, false, 4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(completedRun)
            .Verifiable(Times.Once());

        var rootCommand = new RootCommand();
        var ingestCommand = new Command("ingest");
        ingestCommand.Subcommands.Add(IngestRetryCommand.Build(host));
        rootCommand.Subcommands.Add(ingestCommand);

        // Act
        var exitCode = await rootCommand.Parse(["ingest", "retry", runId.ToString()]).InvokeAsync();

        // Assert
        exitCode.Should().Be(0);
        pipelineMock.VerifyAll();
    }

    [TestMethod]
    public async Task IngestRetry_ShouldExitOne_WhenRunNotFound()
    {
        // Arrange
        var runId = Guid.NewGuid();

        pipelineMock
            .Setup(p => p.RetryAsync(runId, false, 4, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"IngestRun not found (RunId: {runId})."))
            .Verifiable(Times.Once());

        var rootCommand = new RootCommand();
        var ingestCommand = new Command("ingest");
        ingestCommand.Subcommands.Add(IngestRetryCommand.Build(host));
        rootCommand.Subcommands.Add(ingestCommand);

        // Act
        var exitCode = await rootCommand.Parse(["ingest", "retry", runId.ToString()]).InvokeAsync();

        // Assert
        exitCode.Should().Be(1);
        pipelineMock.VerifyAll();
    }

    [TestMethod]
    public async Task IngestRetry_ShouldPassDryRunFlag_WhenProvided()
    {
        // Arrange
        var runId = Guid.NewGuid();
        var completedRun = new IngestRun(Guid.NewGuid(), IngestRunStatus.Completed, "/data", FixedTime, FixedTime);

        pipelineMock
            .Setup(p => p.RetryAsync(runId, true, 4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(completedRun)
            .Verifiable(Times.Once());

        var rootCommand = new RootCommand();
        var ingestCommand = new Command("ingest");
        ingestCommand.Subcommands.Add(IngestRetryCommand.Build(host));
        rootCommand.Subcommands.Add(ingestCommand);

        // Act
        var exitCode = await rootCommand.Parse(["ingest", "retry", runId.ToString(), "--dry-run"]).InvokeAsync();

        // Assert
        exitCode.Should().Be(0);
        pipelineMock.VerifyAll();
    }
}
