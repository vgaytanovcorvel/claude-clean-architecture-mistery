using Microsoft.SemanticKernel;

namespace MisteryApp.Implementation.Ingest;

public class KernelInvoker(Kernel kernel) : IKernelInvoker
{
    public virtual async Task<string> InvokePromptAsync(string prompt, CancellationToken ct = default)
    {
        var result = await kernel.InvokePromptAsync(prompt, cancellationToken: ct);
        return result.ToString();
    }
}
