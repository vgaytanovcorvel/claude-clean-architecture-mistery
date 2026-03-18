namespace MisteryApp.Implementation.Ingest;

public interface IKernelInvoker
{
    Task<string> InvokePromptAsync(string prompt, CancellationToken ct = default);
}
