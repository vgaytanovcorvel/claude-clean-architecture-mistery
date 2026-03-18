using System.CommandLine;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MisteryApp.Abstractions.Ingest.Interfaces;
using MisteryApp.Abstractions.Ingest.Models;
using MisteryApp.Cli.Commands;
using MisteryApp.Common.Enums;

namespace MisteryApp.Cli.Tests.Commands;

[TestClass]
public class IngestRunCommandTests
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
    public async Task IngestRun_ShouldExitZero_WhenNoRejections()
    {
        // Arrange
        const string path = "/data/input";
        var completedRun = new IngestRun(Guid.NewGuid(), IngestRunStatus.Completed, path, FixedTime, FixedTime, 3, 3, 0);

        pipelineMock
            .Setup(p => p.RunAsync(path, false, FileFormat.Auto, 4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(completedRun)
            .Verifiable(Times.Once());

        var rootCommand = new RootCommand();
        var ingestCommand = new Command("ingest");
        ingestCommand.Subcommands.Add(IngestRunCommand.Build(host));
        rootCommand.Subcommands.Add(ingestCommand);

        // Act
        var exitCode = await rootCommand.Parse(["ingest", "run", path]).InvokeAsync();

        // Assert
        exitCode.Should().Be(0);
        pipelineMock.VerifyAll();
    }

    [TestMethod]
    public async Task IngestRun_ShouldExitOne_WhenSomeRejections()
    {
        // Arrange
        const string path = "/data/input";
        var partialRun = new IngestRun(Guid.NewGuid(), IngestRunStatus.PartialSuccess, path, FixedTime, FixedTime, 5, 5, 2);

        pipelineMock
            .Setup(p => p.RunAsync(path, false, FileFormat.Auto, 4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partialRun)
            .Verifiable(Times.Once());

        var rootCommand = new RootCommand();
        var ingestCommand = new Command("ingest");
        ingestCommand.Subcommands.Add(IngestRunCommand.Build(host));
        rootCommand.Subcommands.Add(ingestCommand);

        // Act
        var exitCode = await rootCommand.Parse(["ingest", "run", path]).InvokeAsync();

        // Assert
        exitCode.Should().Be(1);
        pipelineMock.VerifyAll();
    }

    [TestMethod]
    public async Task IngestRun_ShouldPassDryRunFlag_WhenProvided()
    {
        // Arrange
        const string path = "/data/input";
        var completedRun = new IngestRun(Guid.NewGuid(), IngestRunStatus.Completed, path, FixedTime, FixedTime, 1, 1, 0);

        pipelineMock
            .Setup(p => p.RunAsync(path, true, FileFormat.Auto, 4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(completedRun)
            .Verifiable(Times.Once());

        var rootCommand = new RootCommand();
        var ingestCommand = new Command("ingest");
        ingestCommand.Subcommands.Add(IngestRunCommand.Build(host));
        rootCommand.Subcommands.Add(ingestCommand);

        // Act
        var exitCode = await rootCommand.Parse(["ingest", "run", path, "--dry-run"]).InvokeAsync();

        // Assert
        exitCode.Should().Be(0);
        pipelineMock.VerifyAll();
    }

    [TestMethod]
    public async Task IngestRun_ShouldPassFormatOption_WhenFormatIsProvided()
    {
        // Arrange
        const string path = "/data";
        var completedRun = new IngestRun(Guid.NewGuid(), IngestRunStatus.Completed, path, FixedTime, FixedTime, 1, 1, 0);

        pipelineMock
            .Setup(p => p.RunAsync(path, false, FileFormat.Csv, 4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(completedRun)
            .Verifiable(Times.Once());

        var rootCommand = new RootCommand();
        var ingestCommand = new Command("ingest");
        ingestCommand.Subcommands.Add(IngestRunCommand.Build(host));
        rootCommand.Subcommands.Add(ingestCommand);

        // Act
        var exitCode = await rootCommand.Parse(["ingest", "run", path, "--format", "Csv"]).InvokeAsync();

        // Assert
        exitCode.Should().Be(0);
        pipelineMock.VerifyAll();
    }
}
