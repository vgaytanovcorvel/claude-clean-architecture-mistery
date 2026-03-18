using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MisteryApp.Abstractions.Ingest.Interfaces;
using MisteryApp.Abstractions.Ingest.Models;
using MisteryApp.Abstractions.Models;
using MisteryApp.Common.Enums;
using MisteryApp.Implementation.Ingest;

namespace MisteryApp.Implementation.Tests.Ingest;

[TestClass]
public class IngestFileProcessorTests
{
    private Mock<IFileClassifier> dispatcherMock = new(MockBehavior.Strict);
    private Mock<ISchemaMapper> schemaMapperMock = new(MockBehavior.Strict);
    private Mock<IQualityCritic> qualityCriticMock = new(MockBehavior.Strict);
    private Mock<IHealerAgent> healerAgentMock = new(MockBehavior.Strict);
    private Mock<IngestFileProcessor> processorMock = null!;
    private CancellationToken cancellationToken = CancellationToken.None;

    private static readonly MappedRecord SampleRecord = new("rec-1", null, 42m, "test", "sensor", "A");
    private static readonly FileClassification SampleClassification = new("/data/file.csv", FileFormat.Csv);

    [TestInitialize]
    public void Setup()
    {
        processorMock = new Mock<IngestFileProcessor>(
            () => new IngestFileProcessor(
                dispatcherMock.Object,
                schemaMapperMock.Object,
                qualityCriticMock.Object,
                healerAgentMock.Object),
            MockBehavior.Strict);
    }

    [TestMethod]
    public async Task ProcessFileAsync_ShouldReturnSuccess_WhenRecordIsAcceptable()
    {
        // Arrange
        const string filePath = "/data/file.csv";
        var acceptableReport = new QualityReport(SampleRecord, QualityIssueType.None, null, true);

        processorMock
            .Setup(p => p.ProcessFileAsync(filePath, FileFormat.Auto, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        dispatcherMock
            .Setup(d => d.ClassifyAsync(filePath, cancellationToken))
            .ReturnsAsync(SampleClassification)
            .Verifiable(Times.Once());

        schemaMapperMock
            .Setup(m => m.MapAsync(SampleClassification, cancellationToken))
            .ReturnsAsync(SampleRecord)
            .Verifiable(Times.Once());

        qualityCriticMock
            .Setup(q => q.ReviewAsync(SampleRecord, filePath, cancellationToken))
            .ReturnsAsync(acceptableReport)
            .Verifiable(Times.Once());

        // Act
        var result = await processorMock.Object.ProcessFileAsync(filePath, FileFormat.Auto, cancellationToken);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Record.Should().Be(SampleRecord);
        result.RejectionReason.Should().BeNull();
        processorMock.VerifyAll();
        dispatcherMock.VerifyAll();
        schemaMapperMock.VerifyAll();
        qualityCriticMock.VerifyAll();
        healerAgentMock.VerifyAll();
    }

    [TestMethod]
    public async Task ProcessFileAsync_ShouldReturnRejected_WhenHealingFails()
    {
        // Arrange
        const string filePath = "/data/file.log";
        var classification = new FileClassification(filePath, FileFormat.Log);
        var unacceptableReport = new QualityReport(SampleRecord, QualityIssueType.Hallucination, "Fabricated data", false);

        processorMock
            .Setup(p => p.ProcessFileAsync(filePath, FileFormat.Auto, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        dispatcherMock
            .Setup(d => d.ClassifyAsync(filePath, cancellationToken))
            .ReturnsAsync(classification)
            .Verifiable(Times.Once());

        schemaMapperMock
            .Setup(m => m.MapAsync(classification, cancellationToken))
            .ReturnsAsync(SampleRecord)
            .Verifiable(Times.Once());

        qualityCriticMock
            .Setup(q => q.ReviewAsync(SampleRecord, filePath, cancellationToken))
            .ReturnsAsync(unacceptableReport)
            .Verifiable(Times.Once());

        healerAgentMock
            .Setup(h => h.HealAsync(unacceptableReport, cancellationToken))
            .ReturnsAsync(Result<MappedRecord>.Failure("Cannot heal: data is fabricated."))
            .Verifiable(Times.Once());

        // Act
        var result = await processorMock.Object.ProcessFileAsync(filePath, FileFormat.Auto, cancellationToken);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Record.Should().BeNull();
        result.RejectionReason.Should().Contain("fabricated");
        processorMock.VerifyAll();
        dispatcherMock.VerifyAll();
        schemaMapperMock.VerifyAll();
        qualityCriticMock.VerifyAll();
        healerAgentMock.VerifyAll();
    }

    [TestMethod]
    public async Task ProcessFileAsync_ShouldReturnHealedRecord_WhenHealingSucceeds()
    {
        // Arrange
        const string filePath = "/data/file.log";
        var classification = new FileClassification(filePath, FileFormat.Log);
        var healedRecord = SampleRecord with { Value = 10m };
        var unacceptableReport = new QualityReport(SampleRecord, QualityIssueType.NegativeValue, "Negative", false);

        processorMock
            .Setup(p => p.ProcessFileAsync(filePath, FileFormat.Auto, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        dispatcherMock
            .Setup(d => d.ClassifyAsync(filePath, cancellationToken))
            .ReturnsAsync(classification)
            .Verifiable(Times.Once());

        schemaMapperMock
            .Setup(m => m.MapAsync(classification, cancellationToken))
            .ReturnsAsync(SampleRecord)
            .Verifiable(Times.Once());

        qualityCriticMock
            .Setup(q => q.ReviewAsync(SampleRecord, filePath, cancellationToken))
            .ReturnsAsync(unacceptableReport)
            .Verifiable(Times.Once());

        healerAgentMock
            .Setup(h => h.HealAsync(unacceptableReport, cancellationToken))
            .ReturnsAsync(Result<MappedRecord>.Success(healedRecord))
            .Verifiable(Times.Once());

        // Act
        var result = await processorMock.Object.ProcessFileAsync(filePath, FileFormat.Auto, cancellationToken);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Record.Should().Be(healedRecord);
        processorMock.VerifyAll();
        dispatcherMock.VerifyAll();
        schemaMapperMock.VerifyAll();
        qualityCriticMock.VerifyAll();
        healerAgentMock.VerifyAll();
    }

    [TestMethod]
    public async Task ProcessFileAsync_ShouldSkipHealer_WhenQualityReportIsAcceptable()
    {
        // Arrange
        const string filePath = "/data/file.csv";
        var acceptableReport = new QualityReport(SampleRecord, QualityIssueType.None, null, true);

        processorMock
            .Setup(p => p.ProcessFileAsync(filePath, FileFormat.Csv, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        schemaMapperMock
            .Setup(m => m.MapAsync(SampleClassification, cancellationToken))
            .ReturnsAsync(SampleRecord)
            .Verifiable(Times.Once());

        qualityCriticMock
            .Setup(q => q.ReviewAsync(SampleRecord, filePath, cancellationToken))
            .ReturnsAsync(acceptableReport)
            .Verifiable(Times.Once());

        // Act — hintFormat is Csv so dispatcher is NOT called
        var result = await processorMock.Object.ProcessFileAsync(filePath, FileFormat.Csv, cancellationToken);

        // Assert
        result.Succeeded.Should().BeTrue();
        processorMock.VerifyAll();
        dispatcherMock.VerifyAll();   // no calls expected
        schemaMapperMock.VerifyAll();
        qualityCriticMock.VerifyAll();
        healerAgentMock.VerifyAll(); // no calls expected
    }

    [TestMethod]
    public async Task ProcessFileAsync_ShouldUseHintFormat_WhenFormatIsNotAuto()
    {
        // Arrange
        const string filePath = "/data/mystery.bin";
        var classification = new FileClassification(filePath, FileFormat.Log);
        var acceptableReport = new QualityReport(SampleRecord, QualityIssueType.None, null, true);

        processorMock
            .Setup(p => p.ProcessFileAsync(filePath, FileFormat.Log, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        schemaMapperMock
            .Setup(m => m.MapAsync(
                It.Is<FileClassification>(c => c.Format == FileFormat.Log && c.FilePath == filePath),
                cancellationToken))
            .ReturnsAsync(SampleRecord)
            .Verifiable(Times.Once());

        qualityCriticMock
            .Setup(q => q.ReviewAsync(SampleRecord, filePath, cancellationToken))
            .ReturnsAsync(acceptableReport)
            .Verifiable(Times.Once());

        // Act — hint format Log bypasses dispatcher
        var result = await processorMock.Object.ProcessFileAsync(filePath, FileFormat.Log, cancellationToken);

        // Assert
        result.Succeeded.Should().BeTrue();
        processorMock.VerifyAll();
        dispatcherMock.VerifyAll(); // no calls expected
        schemaMapperMock.VerifyAll();
        qualityCriticMock.VerifyAll();
        healerAgentMock.VerifyAll();
    }
}
