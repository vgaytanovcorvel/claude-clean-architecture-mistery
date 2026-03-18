using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MisteryApp.Abstractions.Ingest.Interfaces;
using MisteryApp.Abstractions.Ingest.Models;
using MisteryApp.Common.Enums;
using MisteryApp.Implementation.Ingest.Agents;

namespace MisteryApp.Implementation.Tests.Ingest;

[TestClass]
public class SchemaMapperTests
{
    private Mock<IChatAgentInvoker> chatAgentInvokerMock = new(MockBehavior.Strict);
    private Mock<SchemaMapper> schemaMapperMock = null!;
    private CancellationToken cancellationToken = CancellationToken.None;

    [TestInitialize]
    public void Setup()
    {
        schemaMapperMock = new Mock<SchemaMapper>(
            () => new SchemaMapper(chatAgentInvokerMock.Object),
            MockBehavior.Strict);
    }

    [TestMethod]
    public async Task MapAsync_ShouldReturnMappedRecords_WhenLogFileProvided()
    {
        // Arrange
        var tmpFile = Path.GetTempFileName() + ".log";
        File.WriteAllText(tmpFile, "2024-01-15T10:00:00Z ERROR value=-5.5 source=sensor-1");

        var classification = new FileClassification(tmpFile, FileFormat.Log);
        const string llmResponse = """[{"id":"rec-1","timestamp":"2024-01-15T10:00:00Z","value":-5.5,"description":"ERROR","source":"sensor-1","category":"error"}]""";

        schemaMapperMock
            .Setup(m => m.MapAsync(classification, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        chatAgentInvokerMock
            .Setup(c => c.InvokeChatAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                null,
                cancellationToken))
            .ReturnsAsync(llmResponse)
            .Verifiable(Times.Once());

        try
        {
            // Act
            var result = await schemaMapperMock.Object.MapAsync(classification, cancellationToken);

            // Assert
            result.Should().HaveCount(1);
            result[0].Id.Should().Be("rec-1");
            result[0].Value.Should().Be(-5.5m);
            result[0].Source.Should().Be("sensor-1");

            schemaMapperMock.VerifyAll();
            chatAgentInvokerMock.VerifyAll();
        }
        finally
        {
            File.Delete(tmpFile);
        }
    }

    [TestMethod]
    public async Task MapAsync_ShouldReturnEmptyList_WhenLlmReturnsInvalidJson()
    {
        // Arrange
        var tmpFile = Path.GetTempFileName() + ".log";
        File.WriteAllText(tmpFile, "some log content");

        var classification = new FileClassification(tmpFile, FileFormat.Log);

        schemaMapperMock
            .Setup(m => m.MapAsync(classification, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        chatAgentInvokerMock
            .Setup(c => c.InvokeChatAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                null,
                cancellationToken))
            .ReturnsAsync("not valid json at all")
            .Verifiable(Times.Once());

        try
        {
            // Act
            var result = await schemaMapperMock.Object.MapAsync(classification, cancellationToken);

            // Assert
            result.Should().BeEmpty();

            schemaMapperMock.VerifyAll();
            chatAgentInvokerMock.VerifyAll();
        }
        finally
        {
            File.Delete(tmpFile);
        }
    }

    [TestMethod]
    public async Task MapAsync_ShouldCallLlm_WhenClassificationIsLog()
    {
        // Arrange
        var tmpFile = Path.GetTempFileName() + ".log";
        File.WriteAllText(tmpFile, "log line");

        var classification = new FileClassification(tmpFile, FileFormat.Log);

        schemaMapperMock
            .Setup(m => m.MapAsync(classification, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        chatAgentInvokerMock
            .Setup(c => c.InvokeChatAsync(
                It.IsAny<string>(),
                It.Is<string>(msg => msg.Contains("Log")),
                null,
                cancellationToken))
            .ReturnsAsync("{}")
            .Verifiable(Times.Once());

        try
        {
            // Act
            await schemaMapperMock.Object.MapAsync(classification, cancellationToken);

            // Assert
            schemaMapperMock.VerifyAll();
            chatAgentInvokerMock.VerifyAll();
        }
        finally
        {
            File.Delete(tmpFile);
        }
    }

    [TestMethod]
    public async Task MapAsync_ShouldReturnRecord_WhenValueIsDecimalNumber()
    {
        // Arrange
        var tmpFile = Path.GetTempFileName() + ".log";
        File.WriteAllText(tmpFile, "value: 123.45");

        var classification = new FileClassification(tmpFile, FileFormat.Log);
        const string llmResponse = """{"value":123.45,"description":"test","category":"metrics"}""";

        schemaMapperMock
            .Setup(m => m.MapAsync(classification, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        chatAgentInvokerMock
            .Setup(c => c.InvokeChatAsync(It.IsAny<string>(), It.IsAny<string>(), null, cancellationToken))
            .ReturnsAsync(llmResponse)
            .Verifiable(Times.Once());

        try
        {
            // Act
            var result = await schemaMapperMock.Object.MapAsync(classification, cancellationToken);

            // Assert
            result.Should().HaveCount(1);
            result[0].Value.Should().Be(123.45m);
            result[0].Description.Should().Be("test");

            schemaMapperMock.VerifyAll();
            chatAgentInvokerMock.VerifyAll();
        }
        finally
        {
            File.Delete(tmpFile);
        }
    }
}
