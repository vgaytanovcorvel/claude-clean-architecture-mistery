using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MisteryApp.Common.Ingest;
using MisteryApp.Implementation.Ingest;
using Moq;

namespace MisteryApp.Implementation.Tests.Ingest;

[TestClass]
public class QualityCriticTests
{
    private Mock<IKernelInvoker> kernelInvokerMock = new(MockBehavior.Strict);
    private Mock<QualityCritic> qualityCriticMock = null!;
    private CancellationToken ct = CancellationToken.None;

    [TestInitialize]
    public void Setup()
    {
        qualityCriticMock = new Mock<QualityCritic>(
            () => new QualityCritic(kernelInvokerMock.Object),
            MockBehavior.Strict);
    }

    [TestMethod]
    public async Task CritiqueAsync_ShouldReturnEmptyList_WhenKernelFindsNoIssues()
    {
        // Arrange
        var record = new MappedRecord(
            new Dictionary<string, object?> { ["id"] = "1", ["value"] = "42" },
            []);

        qualityCriticMock
            .Setup(c => c.CritiqueAsync(record, ct))
            .CallBase()
            .Verifiable(Times.Once());

        kernelInvokerMock
            .Setup(k => k.InvokePromptAsync(It.IsAny<string>(), ct))
            .ReturnsAsync("[]")
            .Verifiable(Times.Once());

        // Act
        var result = await qualityCriticMock.Object.CritiqueAsync(record, ct);

        // Assert
        result.Should().BeEmpty();

        qualityCriticMock.VerifyAll();
        kernelInvokerMock.VerifyAll();
    }

    [TestMethod]
    public async Task CritiqueAsync_ShouldReturnIssues_WhenKernelDetectsNegativeValue()
    {
        // Arrange
        var record = new MappedRecord(
            new Dictionary<string, object?> { ["value"] = "-5" },
            []);
        const string issuesJson =
            """[{"fieldName":"value","kind":"NegativeValue","description":"Value is negative"}]""";

        qualityCriticMock
            .Setup(c => c.CritiqueAsync(record, ct))
            .CallBase()
            .Verifiable(Times.Once());

        kernelInvokerMock
            .Setup(k => k.InvokePromptAsync(It.IsAny<string>(), ct))
            .ReturnsAsync(issuesJson)
            .Verifiable(Times.Once());

        // Act
        var result = await qualityCriticMock.Object.CritiqueAsync(record, ct);

        // Assert
        result.Should().HaveCount(1);
        result[0].FieldName.Should().Be("value");
        result[0].Kind.Should().Be(QualityIssueKind.NegativeValue);
        result[0].Description.Should().Be("Value is negative");

        qualityCriticMock.VerifyAll();
        kernelInvokerMock.VerifyAll();
    }

    [TestMethod]
    public async Task CritiqueAsync_ShouldReturnOtherKind_WhenKindIsUnrecognized()
    {
        // Arrange
        var record = new MappedRecord(new Dictionary<string, object?> { ["id"] = "" }, []);
        const string issuesJson =
            """[{"fieldName":"id","kind":"UnknownKind","description":"Some issue"}]""";

        qualityCriticMock
            .Setup(c => c.CritiqueAsync(record, ct))
            .CallBase()
            .Verifiable(Times.Once());

        kernelInvokerMock
            .Setup(k => k.InvokePromptAsync(It.IsAny<string>(), ct))
            .ReturnsAsync(issuesJson)
            .Verifiable(Times.Once());

        // Act
        var result = await qualityCriticMock.Object.CritiqueAsync(record, ct);

        // Assert
        result.Should().HaveCount(1);
        result[0].Kind.Should().Be(QualityIssueKind.Other);

        qualityCriticMock.VerifyAll();
        kernelInvokerMock.VerifyAll();
    }

    [TestMethod]
    public async Task CritiqueAsync_ShouldReturnEmptyList_WhenKernelReturnsNoJsonArray()
    {
        // Arrange
        var record = new MappedRecord(new Dictionary<string, object?> { ["id"] = "1" }, []);

        qualityCriticMock
            .Setup(c => c.CritiqueAsync(record, ct))
            .CallBase()
            .Verifiable(Times.Once());

        kernelInvokerMock
            .Setup(k => k.InvokePromptAsync(It.IsAny<string>(), ct))
            .ReturnsAsync("no array here")
            .Verifiable(Times.Once());

        // Act
        var result = await qualityCriticMock.Object.CritiqueAsync(record, ct);

        // Assert
        result.Should().BeEmpty();

        qualityCriticMock.VerifyAll();
        kernelInvokerMock.VerifyAll();
    }
}
