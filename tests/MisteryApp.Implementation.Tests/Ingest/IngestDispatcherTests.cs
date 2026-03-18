using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MisteryApp.Common.Ingest;
using MisteryApp.Implementation.Ingest;
using Moq;

namespace MisteryApp.Implementation.Tests.Ingest;

[TestClass]
public class IngestDispatcherTests
{
    private Mock<IKernelInvoker> kernelInvokerMock = new(MockBehavior.Strict);
    private Mock<IngestDispatcher> dispatcherMock = null!;
    private CancellationToken ct = CancellationToken.None;

    [TestInitialize]
    public void Setup()
    {
        dispatcherMock = new Mock<IngestDispatcher>(
            () => new IngestDispatcher(kernelInvokerMock.Object),
            MockBehavior.Strict);
    }

    [TestMethod]
    public async Task ClassifyAsync_ShouldReturnRawFileWithDetectedFormat_WhenFileExists()
    {
        // Arrange
        const string filePath = "data.csv";
        const string content = "id,name\n1,test";

        dispatcherMock
            .Setup(d => d.ClassifyAsync(filePath, ct))
            .CallBase()
            .Verifiable(Times.Once());

        dispatcherMock
            .Setup(d => d.ReadFileAsync(filePath, ct))
            .ReturnsAsync(content)
            .Verifiable(Times.Once());

        dispatcherMock
            .Setup(d => d.DetectFormatAsync(filePath, content, ct))
            .ReturnsAsync(IngestFormat.Csv)
            .Verifiable(Times.Once());

        // Act
        var result = await dispatcherMock.Object.ClassifyAsync(filePath, ct);

        // Assert
        result.FilePath.Should().Be(filePath);
        result.RawContent.Should().Be(content);
        result.DetectedFormat.Should().Be(IngestFormat.Csv);

        dispatcherMock.VerifyAll();
        kernelInvokerMock.VerifyAll();
    }

    [TestMethod]
    public async Task ClassifyAsync_ShouldTruncatePreviewTo500Chars_WhenContentExceedsLimit()
    {
        // Arrange
        const string filePath = "big.log";
        var longContent = new string('x', 600);
        var expectedPreview = new string('x', 500);

        dispatcherMock
            .Setup(d => d.ClassifyAsync(filePath, ct))
            .CallBase()
            .Verifiable(Times.Once());

        dispatcherMock
            .Setup(d => d.ReadFileAsync(filePath, ct))
            .ReturnsAsync(longContent)
            .Verifiable(Times.Once());

        dispatcherMock
            .Setup(d => d.DetectFormatAsync(filePath, expectedPreview, ct))
            .ReturnsAsync(IngestFormat.Log)
            .Verifiable(Times.Once());

        // Act
        var result = await dispatcherMock.Object.ClassifyAsync(filePath, ct);

        // Assert
        result.RawContent.Should().HaveLength(600);
        result.DetectedFormat.Should().Be(IngestFormat.Log);

        dispatcherMock.VerifyAll();
    }

    [TestMethod]
    public async Task DetectFormatAsync_ShouldReturnCsv_WhenFileHasCsvExtension()
    {
        // Arrange
        const string filePath = "data.csv";
        const string preview = "id,name";

        dispatcherMock
            .Setup(d => d.DetectFormatAsync(filePath, preview, ct))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        var result = await dispatcherMock.Object.DetectFormatAsync(filePath, preview, ct);

        // Assert
        result.Should().Be(IngestFormat.Csv);

        dispatcherMock.VerifyAll();
        kernelInvokerMock.VerifyAll();
    }

    [TestMethod]
    public async Task DetectFormatAsync_ShouldCallKernelAndReturnLog_WhenExtensionIsUnknown()
    {
        // Arrange
        const string filePath = "data.dat";
        const string preview = "2024-01-01 ERROR something failed";

        kernelInvokerMock
            .Setup(k => k.InvokePromptAsync(It.IsAny<string>(), ct))
            .ReturnsAsync("log")
            .Verifiable(Times.Once());

        dispatcherMock
            .Setup(d => d.DetectFormatAsync(filePath, preview, ct))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        var result = await dispatcherMock.Object.DetectFormatAsync(filePath, preview, ct);

        // Assert
        result.Should().Be(IngestFormat.Log);

        dispatcherMock.VerifyAll();
        kernelInvokerMock.VerifyAll();
    }

    [TestMethod]
    public async Task DetectFormatAsync_ShouldReturnUnknown_WhenKernelResponseIsUnrecognized()
    {
        // Arrange
        const string filePath = "data.bin";
        const string preview = "binary data here";

        kernelInvokerMock
            .Setup(k => k.InvokePromptAsync(It.IsAny<string>(), ct))
            .ReturnsAsync("xml")
            .Verifiable(Times.Once());

        dispatcherMock
            .Setup(d => d.DetectFormatAsync(filePath, preview, ct))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        var result = await dispatcherMock.Object.DetectFormatAsync(filePath, preview, ct);

        // Assert
        result.Should().Be(IngestFormat.Unknown);

        dispatcherMock.VerifyAll();
        kernelInvokerMock.VerifyAll();
    }
}
