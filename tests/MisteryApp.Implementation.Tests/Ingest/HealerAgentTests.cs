using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MisteryApp.Abstractions.Ingest.Interfaces;
using MisteryApp.Abstractions.Ingest.Models;
using MisteryApp.Common.Enums;
using MisteryApp.Implementation.Ingest.Agents;

namespace MisteryApp.Implementation.Tests.Ingest;

[TestClass]
public class HealerAgentTests
{
    private Mock<IChatAgentInvoker> chatAgentInvokerMock = new(MockBehavior.Strict);
    private Mock<HealerAgent> healerAgentMock = null!;
    private CancellationToken cancellationToken = CancellationToken.None;
    private static readonly MappedRecord SampleRecord = new("rec-1", null, -10m, "test", "sensor", "A");

    [TestInitialize]
    public void Setup()
    {
        healerAgentMock = new Mock<HealerAgent>(
            () => new HealerAgent(chatAgentInvokerMock.Object),
            MockBehavior.Strict);
    }

    [TestMethod]
    public async Task HealAsync_ShouldReturnSuccess_WhenIssueTypeIsNone()
    {
        // Arrange
        var report = new QualityReport(SampleRecord, QualityIssueType.None, null, true);

        healerAgentMock
            .Setup(h => h.HealAsync(report, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        var result = await healerAgentMock.Object.HealAsync(report, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(SampleRecord);
        healerAgentMock.VerifyAll();
        chatAgentInvokerMock.VerifyAll();
    }

    [TestMethod]
    public async Task HealAsync_ShouldReturnHealedRecord_WhenRecordIsHealable()
    {
        // Arrange
        var report = new QualityReport(SampleRecord, QualityIssueType.NegativeValue, "Value is negative: -10", false);
        const string proposedFix = """{"id":"rec-1","value":10,"description":"test","source":"sensor","category":"A"}""";
        const string validatedFix = """{"id":"rec-1","value":10,"description":"test","source":"sensor","category":"A"}""";

        healerAgentMock
            .Setup(h => h.HealAsync(report, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        chatAgentInvokerMock
            .Setup(c => c.InvokeChatAsync(It.IsAny<string>(), It.IsAny<string>(), null, cancellationToken))
            .ReturnsAsync(proposedFix)
            .Verifiable(Times.Once());

        chatAgentInvokerMock
            .Setup(c => c.InvokeChatAsync(
                It.IsAny<string>(),
                It.Is<string>(m => m.Contains("Validate")),
                It.IsNotNull<IReadOnlyList<(string, string)>>(),
                cancellationToken))
            .ReturnsAsync(validatedFix)
            .Verifiable(Times.Once());

        // Act
        var result = await healerAgentMock.Object.HealAsync(report, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Value.Should().Be(10m);
        healerAgentMock.VerifyAll();
        chatAgentInvokerMock.VerifyAll();
    }

    [TestMethod]
    public async Task HealAsync_ShouldReturnFailure_WhenRecordIsUnhealable()
    {
        // Arrange
        var report = new QualityReport(SampleRecord, QualityIssueType.Hallucination, "Data appears fabricated", false);
        const string cannotHealResponse = """{"cannotHeal":true,"reason":"Data is completely fabricated and cannot be corrected."}""";

        healerAgentMock
            .Setup(h => h.HealAsync(report, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        chatAgentInvokerMock
            .Setup(c => c.InvokeChatAsync(It.IsAny<string>(), It.IsAny<string>(), null, cancellationToken))
            .ReturnsAsync(cannotHealResponse)
            .Verifiable(Times.Once());

        // Act
        var result = await healerAgentMock.Object.HealAsync(report, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("fabricated");
        healerAgentMock.VerifyAll();
        chatAgentInvokerMock.VerifyAll();
    }

    [TestMethod]
    public async Task HealAsync_ShouldTreatNegativeValueAsRefund_WhenValidationConfirms()
    {
        // Arrange
        var negativeRecord = SampleRecord with { Value = -50m, Category = "refund" };
        var report = new QualityReport(negativeRecord, QualityIssueType.NegativeValue, "Value is negative: -50", false);
        const string proposedFix = """{"id":"rec-1","value":-50,"description":"test","source":"sensor","category":"refund"}""";
        const string validatedFix = """{"id":"rec-1","value":-50,"description":"test","source":"sensor","category":"refund"}""";

        healerAgentMock
            .Setup(h => h.HealAsync(report, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        chatAgentInvokerMock
            .Setup(c => c.InvokeChatAsync(It.IsAny<string>(), It.IsAny<string>(), null, cancellationToken))
            .ReturnsAsync(proposedFix)
            .Verifiable(Times.Once());

        chatAgentInvokerMock
            .Setup(c => c.InvokeChatAsync(
                It.IsAny<string>(),
                It.Is<string>(m => m.Contains("Validate")),
                It.IsNotNull<IReadOnlyList<(string, string)>>(),
                cancellationToken))
            .ReturnsAsync(validatedFix)
            .Verifiable(Times.Once());

        // Act
        var result = await healerAgentMock.Object.HealAsync(report, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Value.Should().Be(-50m);
        healerAgentMock.VerifyAll();
        chatAgentInvokerMock.VerifyAll();
    }
}
