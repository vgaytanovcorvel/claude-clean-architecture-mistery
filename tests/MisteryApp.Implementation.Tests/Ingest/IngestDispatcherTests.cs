using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MisteryApp.Abstractions.Ingest.Interfaces;
using MisteryApp.Common.Enums;
using MisteryApp.Implementation.Ingest.Agents;

namespace MisteryApp.Implementation.Tests.Ingest;

[TestClass]
public class IngestDispatcherTests
{
    private Mock<IChatAgentInvoker> chatAgentInvokerMock = new(MockBehavior.Strict);
    private Mock<IngestDispatcher> ingestDispatcherMock = null!;
    private CancellationToken cancellationToken = CancellationToken.None;

    [TestInitialize]
    public void Setup()
    {
        ingestDispatcherMock = new Mock<IngestDispatcher>(
            () => new IngestDispatcher(chatAgentInvokerMock.Object),
            MockBehavior.Strict);
    }

    [TestMethod]
    public async Task ClassifyAsync_ShouldReturnCsv_WhenExtensionIsCsv()
    {
        // Arrange
        const string filePath = "/data/sales.csv";

        ingestDispatcherMock
            .Setup(d => d.ClassifyAsync(filePath, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        var result = await ingestDispatcherMock.Object.ClassifyAsync(filePath, cancellationToken);

        // Assert
        result.Format.Should().Be(FileFormat.Csv);
        result.FilePath.Should().Be(filePath);
        ingestDispatcherMock.VerifyAll();
        chatAgentInvokerMock.VerifyAll();
    }

    [TestMethod]
    public async Task ClassifyAsync_ShouldReturnLog_WhenExtensionIsLog()
    {
        // Arrange
        const string filePath = "/data/app.log";

        ingestDispatcherMock
            .Setup(d => d.ClassifyAsync(filePath, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        var result = await ingestDispatcherMock.Object.ClassifyAsync(filePath, cancellationToken);

        // Assert
        result.Format.Should().Be(FileFormat.Log);
        ingestDispatcherMock.VerifyAll();
        chatAgentInvokerMock.VerifyAll();
    }

    [TestMethod]
    public async Task ClassifyAsync_ShouldCallLlmAndReturnFormat_WhenExtensionUnknown()
    {
        // Arrange
        const string filePath = "/data/report.xyz";

        ingestDispatcherMock
            .Setup(d => d.ClassifyAsync(filePath, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        chatAgentInvokerMock
            .Setup(c => c.InvokeChatAsync(
                It.IsAny<string>(),
                It.Is<string>(m => m.Contains(filePath)),
                null,
                cancellationToken))
            .ReturnsAsync("Csv")
            .Verifiable(Times.Once());

        // Act
        var result = await ingestDispatcherMock.Object.ClassifyAsync(filePath, cancellationToken);

        // Assert
        result.Format.Should().Be(FileFormat.Csv);
        ingestDispatcherMock.VerifyAll();
        chatAgentInvokerMock.VerifyAll();
    }

    [TestMethod]
    public async Task ClassifyAsync_ShouldReturnUnknown_WhenLlmReturnsUnknown()
    {
        // Arrange
        const string filePath = "/data/mystery.bin";

        ingestDispatcherMock
            .Setup(d => d.ClassifyAsync(filePath, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        chatAgentInvokerMock
            .Setup(c => c.InvokeChatAsync(
                It.IsAny<string>(),
                It.Is<string>(m => m.Contains(filePath)),
                null,
                cancellationToken))
            .ReturnsAsync("Unknown")
            .Verifiable(Times.Once());

        // Act
        var result = await ingestDispatcherMock.Object.ClassifyAsync(filePath, cancellationToken);

        // Assert
        result.Format.Should().Be(FileFormat.Unknown);
        ingestDispatcherMock.VerifyAll();
        chatAgentInvokerMock.VerifyAll();
    }
}
