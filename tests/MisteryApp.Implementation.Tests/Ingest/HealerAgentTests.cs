using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MisteryApp.Common.Ingest;
using MisteryApp.Implementation.Ingest;
using Moq;

namespace MisteryApp.Implementation.Tests.Ingest;

[TestClass]
public class HealerAgentTests
{
    private Mock<IKernelInvoker> kernelInvokerMock = new(MockBehavior.Strict);
    private Mock<HealerAgent> healerAgentMock = null!;
    private CancellationToken ct = CancellationToken.None;

    [TestInitialize]
    public void Setup()
    {
        healerAgentMock = new Mock<HealerAgent>(
            () => new HealerAgent(kernelInvokerMock.Object),
            MockBehavior.Strict);
    }

    [TestMethod]
    public async Task HealAsync_ShouldReturnHealedRecord_WhenIssuesCanBeFixed()
    {
        // Arrange
        var record = new MappedRecord(
            new Dictionary<string, object?> { ["value"] = "-5" },
            ["CSV parsed"]);
        var issues = new List<QualityIssue>
        {
            new("value", QualityIssueKind.NegativeValue, "Value is negative")
        };
        const string healResponse =
            """{"success":true,"fields":{"value":"5"},"healingNotes":["Fixed negative value to absolute"]}""";

        healerAgentMock
            .Setup(h => h.HealAsync(record, issues, ct))
            .CallBase()
            .Verifiable(Times.Once());

        kernelInvokerMock
            .Setup(k => k.InvokePromptAsync(It.IsAny<string>(), ct))
            .ReturnsAsync(healResponse)
            .Verifiable(Times.Once());

        // Act
        var result = await healerAgentMock.Object.HealAsync(record, issues, ct);

        // Assert
        result.Should().NotBeNull();
        result!.Record.Fields.Should().ContainKey("value");
        result.HealingNotes.Should().ContainSingle().Which.Should().Contain("negative");
        result.Record.AppliedTransformations.Should().ContainSingle().Which.Should().Be("CSV parsed");

        healerAgentMock.VerifyAll();
        kernelInvokerMock.VerifyAll();
    }

    [TestMethod]
    public async Task HealAsync_ShouldReturnNull_WhenIssuesCannotBeFixed()
    {
        // Arrange
        var record = new MappedRecord(
            new Dictionary<string, object?> { ["value"] = "???" },
            []);
        var issues = new List<QualityIssue>
        {
            new("value", QualityIssueKind.Hallucination, "Cannot determine real value")
        };
        const string healResponse = """{"success":false,"fields":{},"healingNotes":[]}""";

        healerAgentMock
            .Setup(h => h.HealAsync(record, issues, ct))
            .CallBase()
            .Verifiable(Times.Once());

        kernelInvokerMock
            .Setup(k => k.InvokePromptAsync(It.IsAny<string>(), ct))
            .ReturnsAsync(healResponse)
            .Verifiable(Times.Once());

        // Act
        var result = await healerAgentMock.Object.HealAsync(record, issues, ct);

        // Assert
        result.Should().BeNull();

        healerAgentMock.VerifyAll();
        kernelInvokerMock.VerifyAll();
    }

    [TestMethod]
    public async Task HealAsync_ShouldReturnNull_WhenKernelReturnsNoJson()
    {
        // Arrange
        var record = new MappedRecord(new Dictionary<string, object?> { ["id"] = "" }, []);
        var issues = new List<QualityIssue>
        {
            new("id", QualityIssueKind.MissingRequired, "ID is empty")
        };

        healerAgentMock
            .Setup(h => h.HealAsync(record, issues, ct))
            .CallBase()
            .Verifiable(Times.Once());

        kernelInvokerMock
            .Setup(k => k.InvokePromptAsync(It.IsAny<string>(), ct))
            .ReturnsAsync("I cannot fix this record.")
            .Verifiable(Times.Once());

        // Act
        var result = await healerAgentMock.Object.HealAsync(record, issues, ct);

        // Assert
        result.Should().BeNull();

        healerAgentMock.VerifyAll();
        kernelInvokerMock.VerifyAll();
    }
}
