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
public class IngestListCommandTests
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
    public async Task IngestList_ShouldExitZero_WhenRunsExist()
    {
        // Arrange
        var summaries = new List<IngestRunSummary>
        {
            new(Guid.NewGuid(), IngestRunStatus.Completed, "/data", FixedTime, FixedTime, 5, 5, 0)
        };

        pipelineMock
            .Setup(r => r.GetRunSummariesAsync(20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(summaries)
            .Verifiable(Times.Once());

        var rootCommand = new RootCommand();
        var ingestCommand = new Command("ingest");
        ingestCommand.Subcommands.Add(IngestListCommand.Build(host));
        rootCommand.Subcommands.Add(ingestCommand);

        // Act
        var exitCode = await rootCommand.Parse(["ingest", "list"]).InvokeAsync();

        // Assert
        exitCode.Should().Be(0);
        pipelineMock.VerifyAll();
    }

    [TestMethod]
    public async Task IngestList_ShouldRespectLimitOption_WhenProvided()
    {
        // Arrange
        var summaries = new List<IngestRunSummary>();

        pipelineMock
            .Setup(r => r.GetRunSummariesAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(summaries)
            .Verifiable(Times.Once());

        var rootCommand = new RootCommand();
        var ingestCommand = new Command("ingest");
        ingestCommand.Subcommands.Add(IngestListCommand.Build(host));
        rootCommand.Subcommands.Add(ingestCommand);

        // Act
        var exitCode = await rootCommand.Parse(["ingest", "list", "--limit", "5"]).InvokeAsync();

        // Assert
        exitCode.Should().Be(0);
        pipelineMock.VerifyAll();
    }
}
