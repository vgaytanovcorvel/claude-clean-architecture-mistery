using MisteryApp.Abstractions.Ingest;
using MisteryApp.Common.Ingest;

namespace MisteryApp.Implementation.Ingest;

public class IngestDispatcher(IKernelInvoker kernelInvoker) : IIngestDispatcher
{
    private const int PreviewLength = 500;

    public virtual async Task<RawFile> ClassifyAsync(string filePath, CancellationToken ct = default)
    {
        var content = await ReadFileAsync(filePath, ct);
        var preview = content.Length > PreviewLength ? content[..PreviewLength] : content;
        var format = await DetectFormatAsync(filePath, preview, ct);
        return new RawFile(filePath, content, format);
    }

    internal virtual Task<string> ReadFileAsync(string filePath, CancellationToken ct)
        => File.ReadAllTextAsync(filePath, ct);

    internal virtual async Task<IngestFormat> DetectFormatAsync(
        string filePath, string preview, CancellationToken ct)
    {
        var ext = Path.GetExtension(filePath).ToLowerInvariant();
        var extFormat = ParseExtensionFormat(ext);
        if (extFormat != IngestFormat.Auto)
            return extFormat;

        var prompt = BuildClassifyPrompt(filePath, preview);
        var response = await kernelInvoker.InvokePromptAsync(prompt, ct);
        return ParseFormatResponse(response.Trim().ToLowerInvariant());
    }

    private static IngestFormat ParseExtensionFormat(string ext) => ext switch
    {
        ".csv" => IngestFormat.Csv,
        ".log" => IngestFormat.Log,
        ".txt" => IngestFormat.Log,
        _ => IngestFormat.Auto
    };

    private static IngestFormat ParseFormatResponse(string response) => response switch
    {
        "csv" => IngestFormat.Csv,
        "log" => IngestFormat.Log,
        "ocr" => IngestFormat.OcrText,
        _ => IngestFormat.Unknown
    };

    private static string BuildClassifyPrompt(string filePath, string preview) =>
        $"""
        Classify the format of this file. Reply with exactly one word: csv, log, ocr, or unknown.
        File: {filePath}
        Content:
        {preview}
        Format:
        """;
}
