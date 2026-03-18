using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MisteryApp.Implementation.Ingest;

namespace MisteryApp.Implementation.Tests.Ingest;

[TestClass]
public class ChatAgentInvokerTests
{
    private Mock<IChatCompletionService> chatCompletionServiceMock = new(MockBehavior.Strict);
    private Kernel kernel = null!;
    private Mock<ChatAgentInvoker> chatAgentInvokerMock = null!;
    private CancellationToken cancellationToken = CancellationToken.None;

    [TestInitialize]
    public void Setup()
    {
        var builder = Kernel.CreateBuilder();
        builder.Services.AddSingleton(chatCompletionServiceMock.Object);
        kernel = builder.Build();

        chatAgentInvokerMock = new Mock<ChatAgentInvoker>(
            () => new ChatAgentInvoker(kernel),
            MockBehavior.Strict);
    }

    [TestMethod]
    public async Task InvokeChatAsync_ShouldReturnResponse_WhenNoHistory()
    {
        // Arrange
        const string systemPrompt = "You are an assistant.";
        const string userMessage = "Hello";
        const string expectedResponse = "Hi there!";

        SetupChatCompletion(expectedResponse);

        chatAgentInvokerMock
            .Setup(x => x.InvokeChatAsync(systemPrompt, userMessage, null, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        var result = await chatAgentInvokerMock.Object.InvokeChatAsync(systemPrompt, userMessage, null, cancellationToken);

        // Assert
        result.Should().Be(expectedResponse);
        chatAgentInvokerMock.VerifyAll();
        chatCompletionServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task InvokeChatAsync_ShouldReplayHistory_WhenHistoryProvided()
    {
        // Arrange
        const string systemPrompt = "You are an assistant.";
        const string userMessage = "Follow up question";
        const string expectedResponse = "Follow up answer";
        var history = new List<(string Role, string Content)>
        {
            ("user", "First question"),
            ("assistant", "First answer")
        };

        SetupChatCompletion(expectedResponse);

        chatAgentInvokerMock
            .Setup(x => x.InvokeChatAsync(systemPrompt, userMessage, history, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        var result = await chatAgentInvokerMock.Object.InvokeChatAsync(systemPrompt, userMessage, history, cancellationToken);

        // Assert
        result.Should().Be(expectedResponse);
        chatAgentInvokerMock.VerifyAll();
        chatCompletionServiceMock.VerifyAll();
    }

    [TestMethod]
    public async Task InvokeChatAsync_ShouldHandleEmptyResponse_WhenNullHistory()
    {
        // Arrange
        const string systemPrompt = "You are an assistant.";
        const string userMessage = "Question";

        SetupChatCompletion(string.Empty);

        chatAgentInvokerMock
            .Setup(x => x.InvokeChatAsync(systemPrompt, userMessage, null, cancellationToken))
            .CallBase()
            .Verifiable(Times.Once());

        // Act
        var result = await chatAgentInvokerMock.Object.InvokeChatAsync(systemPrompt, userMessage, null, cancellationToken);

        // Assert
        result.Should().BeEmpty();
        chatAgentInvokerMock.VerifyAll();
        chatCompletionServiceMock.VerifyAll();
    }

    private void SetupChatCompletion(string responseContent)
    {
        chatCompletionServiceMock
            .Setup(s => s.GetChatMessageContentsAsync(
                It.IsAny<ChatHistory>(),
                It.IsAny<PromptExecutionSettings?>(),
                It.IsAny<Kernel?>(),
                cancellationToken))
            .ReturnsAsync([new ChatMessageContent(AuthorRole.Assistant, responseContent)])
            .Verifiable(Times.Once());
    }
}
