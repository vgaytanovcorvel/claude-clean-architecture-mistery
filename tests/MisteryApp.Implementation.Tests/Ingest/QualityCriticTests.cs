using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MisteryApp.Abstractions.Ingest.Interfaces;
using MisteryApp.Abstractions.Ingest.Models;
using MisteryApp.Common.Enums;
using MisteryApp.Implementation.Ingest.Agents;

namespace MisteryApp.Implementation.Tests.Ingest;

[TestClass]
public class QualityCriticTests
{
    private Mock<IChatAgentInvoker> chatAgentInvokerMock = new(MockBehavior.Strict);
    private Mock<QualityCritic> qualityCriticMock = null!;
    private CancellationToken cancellationToken = CancellationToken.None;
    private static readonly MappedRecord SampleRecord = new("rec-1", null, 42m, "test", "sensor", "A");

    [TestInitialize]
    public void Setup()
    {
        qualityCriticMock = new Mock<QualityCritic>(
            () => new QualityCritic(chatAgentInvokerMock.Object),
            MockBehavior.Strict);
    }

    [TestMethod]
    public async Task ReviewAsync_ShouldReturnAcceptable_WhenRecordHasNoIssues()
    {
        // Arrange
        const string filePath = "/data/file.log";
        const string llmResponse = """{"issueType":"None","issueDetail":null,"isAcceptable":true}""";

        qualityCriticMock
            .Setup(q => q.ReviewAsync(SampleRecord, filePath, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        chatAgentInvokerMock
            .Setup(c => c.InvokeChatAsync(It.IsAny<string>(), It.IsAny<string>(), null, cancellationToken))
            .ReturnsAsync(llmResponse)
            .Verifiable(Times.Once());

        // Act
        var result = await qualityCriticMock.Object.ReviewAsync(SampleRecord, filePath, cancellationToken);

        // Assert
        result.IsAcceptable.Should().BeTrue();
        result.IssueType.Should().Be(QualityIssueType.None);
        qualityCriticMock.VerifyAll();
        chatAgentInvokerMock.VerifyAll();
    }

    [TestMethod]
    public async Task ReviewAsync_ShouldReturnNegativeValue_WhenValueIsNegative()
    {
        // Arrange
        const string filePath = "/data/file.log";
        var recordWithNegativeValue = SampleRecord with { Value = -100m };
        const string llmResponse = """{"issueType":"NegativeValue","issueDetail":"Value is negative: -100","isAcceptable":false}""";

        qualityCriticMock
            .Setup(q => q.ReviewAsync(recordWithNegativeValue, filePath, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        chatAgentInvokerMock
            .Setup(c => c.InvokeChatAsync(It.IsAny<string>(), It.IsAny<string>(), null, cancellationToken))
            .ReturnsAsync(llmResponse)
            .Verifiable(Times.Once());

        // Act
        var result = await qualityCriticMock.Object.ReviewAsync(recordWithNegativeValue, filePath, cancellationToken);

        // Assert
        result.IsAcceptable.Should().BeFalse();
        result.IssueType.Should().Be(QualityIssueType.NegativeValue);
        result.IssueDetail.Should().Contain("-100");
        qualityCriticMock.VerifyAll();
        chatAgentInvokerMock.VerifyAll();
    }

    [TestMethod]
    public async Task ReviewAsync_ShouldReturnMissingRequired_WhenRequiredFieldMissing()
    {
        // Arrange
        const string filePath = "/data/file.log";
        var recordMissingFields = new MappedRecord(null, null, null, null, null, null);
        const string llmResponse = """{"issueType":"MissingRequired","issueDetail":"Id and timestamp are missing","isAcceptable":false}""";

        qualityCriticMock
            .Setup(q => q.ReviewAsync(recordMissingFields, filePath, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        chatAgentInvokerMock
            .Setup(c => c.InvokeChatAsync(It.IsAny<string>(), It.IsAny<string>(), null, cancellationToken))
            .ReturnsAsync(llmResponse)
            .Verifiable(Times.Once());

        // Act
        var result = await qualityCriticMock.Object.ReviewAsync(recordMissingFields, filePath, cancellationToken);

        // Assert
        result.IsAcceptable.Should().BeFalse();
        result.IssueType.Should().Be(QualityIssueType.MissingRequired);
        qualityCriticMock.VerifyAll();
        chatAgentInvokerMock.VerifyAll();
    }

    [TestMethod]
    public async Task ReviewAsync_ShouldRequestMetadataAndRetry_WhenLlmRequestsMetadata()
    {
        // Arrange
        const string filePath = "/data/file.log";
        const string firstResponse = "REQUEST_METADATA — need file info to complete review";
        const string secondResponse = """{"issueType":"None","issueDetail":null,"isAcceptable":true}""";
        const string fileMetadata = "Name: file.log, Size: 1024 bytes, LastModified: 2024-01-15T10:00:00.0000000Z";

        qualityCriticMock
            .Setup(q => q.ReviewAsync(SampleRecord, filePath, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        qualityCriticMock
            .Setup(q => q.GetFileMetadata(filePath))
            .Returns(fileMetadata)
            .Verifiable(Times.Once());

        chatAgentInvokerMock
            .Setup(c => c.InvokeChatAsync(It.IsAny<string>(), It.IsAny<string>(), null, cancellationToken))
            .ReturnsAsync(firstResponse)
            .Verifiable(Times.Once());

        chatAgentInvokerMock
            .Setup(c => c.InvokeChatAsync(
                It.IsAny<string>(),
                It.Is<string>(m => m.StartsWith("File metadata:")),
                It.IsNotNull<IReadOnlyList<(string, string)>>(),
                cancellationToken))
            .ReturnsAsync(secondResponse)
            .Verifiable(Times.Once());

        // Act
        var result = await qualityCriticMock.Object.ReviewAsync(SampleRecord, filePath, cancellationToken);

        // Assert
        result.IsAcceptable.Should().BeTrue();
        result.IssueType.Should().Be(QualityIssueType.None);
        qualityCriticMock.VerifyAll();
        chatAgentInvokerMock.VerifyAll();
    }
}
