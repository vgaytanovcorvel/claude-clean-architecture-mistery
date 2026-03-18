using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MisteryApp.Common.Ingest;
using MisteryApp.Implementation.Ingest;
using Moq;

namespace MisteryApp.Implementation.Tests.Ingest;

[TestClass]
public class SchemaMapperTests
{
    private Mock<IKernelInvoker> kernelInvokerMock = new(MockBehavior.Strict);
    private Mock<SchemaMapper> schemaMapperMock = null!;
    private CancellationToken ct = CancellationToken.None;

    [TestInitialize]
    public void Setup()
    {
        schemaMapperMock = new Mock<SchemaMapper>(
            () => new SchemaMapper(kernelInvokerMock.Object),
            MockBehavior.Strict);
    }

    [TestMethod]
    public async Task MapAsync_ShouldReturnMappedRecord_WhenKernelReturnsValidJson()
    {
        // Arrange
        var rawFile = new RawFile("test.log", "some log content", IngestFormat.Log);
        const string kernelResponse = """{"id": "1", "description": "test event"}""";

        schemaMapperMock
            .Setup(m => m.MapAsync(rawFile, ct))
            .CallBase()
            .Verifiable(Times.Once());

        schemaMapperMock
            .Setup(m => m.PrepareContent(rawFile, It.IsAny<List<string>>()))
            .Returns("some log content")
            .Verifiable(Times.Once());

        kernelInvokerMock
            .Setup(k => k.InvokePromptAsync(It.IsAny<string>(), ct))
            .ReturnsAsync(kernelResponse)
            .Verifiable(Times.Once());

        // Act
        var result = await schemaMapperMock.Object.MapAsync(rawFile, ct);

        // Assert
        result.Should().NotBeNull();
        result.Fields.Should().ContainKey("id");
        result.Fields.Should().ContainKey("description");

        schemaMapperMock.VerifyAll();
        kernelInvokerMock.VerifyAll();
    }

    [TestMethod]
    public async Task MapAsync_ShouldReturnEmptyFields_WhenKernelReturnsNoJson()
    {
        // Arrange
        var rawFile = new RawFile("test.log", "content", IngestFormat.Log);

        schemaMapperMock
            .Setup(m => m.MapAsync(rawFile, ct))
            .CallBase()
            .Verifiable(Times.Once());

        schemaMapperMock
            .Setup(m => m.PrepareContent(rawFile, It.IsAny<List<string>>()))
            .Returns("content")
            .Verifiable(Times.Once());

        kernelInvokerMock
            .Setup(k => k.InvokePromptAsync(It.IsAny<string>(), ct))
            .ReturnsAsync("no json here")
            .Verifiable(Times.Once());

        // Act
        var result = await schemaMapperMock.Object.MapAsync(rawFile, ct);

        // Assert
        result.Fields.Should().BeEmpty();

        schemaMapperMock.VerifyAll();
        kernelInvokerMock.VerifyAll();
    }

    [TestMethod]
    public void PrepareContent_ShouldAddTransformation_WhenFormatIsCsv()
    {
        // Arrange
        var rawFile = new RawFile("data.csv", "id,name\n1,Alice", IngestFormat.Csv);
        var transformations = new List<string>();

        schemaMapperMock
            .Setup(m => m.PrepareContent(rawFile, transformations))
            .CallBase()
            .Verifiable(Times.Once());

        schemaMapperMock
            .Setup(m => m.ParseCsvContent(rawFile.RawContent))
            .Returns("""[{"id":"1","name":"Alice"}]""")
            .Verifiable(Times.Once());

        // Act
        var result = schemaMapperMock.Object.PrepareContent(rawFile, transformations);

        // Assert
        result.Should().Contain("Alice");
        transformations.Should().ContainSingle();

        schemaMapperMock.VerifyAll();
    }

    [TestMethod]
    public void PrepareContent_ShouldReturnRawContent_WhenFormatIsNotCsv()
    {
        // Arrange
        const string content = "log line 1\nlog line 2";
        var rawFile = new RawFile("app.log", content, IngestFormat.Log);
        var transformations = new List<string>();

        schemaMapperMock
            .Setup(m => m.PrepareContent(rawFile, transformations))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        var result = schemaMapperMock.Object.PrepareContent(rawFile, transformations);

        // Assert
        result.Should().Be(content);
        transformations.Should().BeEmpty();

        schemaMapperMock.VerifyAll();
        kernelInvokerMock.VerifyAll();
    }
}
