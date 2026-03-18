using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using MisteryApp.Abstractions.Ingest.Interfaces;

namespace MisteryApp.Implementation.Ingest;

public class ChatAgentInvoker(Kernel kernel) : IChatAgentInvoker
{
    public virtual async Task<string> InvokeChatAsync(
        string systemPrompt,
        string userMessage,
        IReadOnlyList<(string Role, string Content)>? history,
        CancellationToken cancellationToken)
    {
        var agent = new ChatCompletionAgent
        {
            Kernel = kernel,
            Name = "IngestAgent",
            Instructions = systemPrompt,
            Arguments = new KernelArguments(new OpenAIPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            })
        };

        var priorHistory = new ChatHistory();
        foreach (var (role, content) in history ?? [])
        {
            var authorRole = role.ToLowerInvariant() switch
            {
                "user" => AuthorRole.User,
                "system" => AuthorRole.System,
                "assistant" => AuthorRole.Assistant,
                _ => throw new ArgumentOutOfRangeException(nameof(history), $"Unknown role: {role}")
            };
            priorHistory.Add(new ChatMessageContent(authorRole, content));
        }

        AgentThread thread = new ChatHistoryAgentThread(priorHistory);
        var sb = new System.Text.StringBuilder();
        var userMsg = new ChatMessageContent(AuthorRole.User, userMessage);

        await foreach (var item in agent.InvokeAsync(userMsg, thread, cancellationToken: cancellationToken))
        {
            if (item.Message.Content is not null)
                sb.Append(item.Message.Content);
        }
        return sb.ToString();
    }
}
