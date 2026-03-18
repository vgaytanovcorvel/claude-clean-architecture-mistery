namespace MisteryApp.Abstractions.Ingest.Interfaces;

public interface IChatAgentInvoker
{
    Task<string> InvokeChatAsync(
        string systemPrompt,
        string userMessage,
        IReadOnlyList<(string Role, string Content)>? history,
        CancellationToken cancellationToken);
}
