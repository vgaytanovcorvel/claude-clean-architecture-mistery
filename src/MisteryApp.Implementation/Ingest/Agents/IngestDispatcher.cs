using MisteryApp.Abstractions.Ingest.Interfaces;
using MisteryApp.Abstractions.Ingest.Models;
using MisteryApp.Common.Enums;

namespace MisteryApp.Implementation.Ingest.Agents;

public class IngestDispatcher(IChatAgentInvoker chatAgentInvoker) : IFileClassifier
{
    private const string ClassifySystemPrompt = """
        You are a file format classifier. Given a file path, classify its format.
        Respond with exactly one word: Csv, Log, Ocr, or Unknown.
        """;

    public virtual async Task<FileClassification> ClassifyAsync(
        string filePath,
        CancellationToken cancellationToken)
    {
        var formatByExtension = DetectByExtension(filePath);
        if (formatByExtension != FileFormat.Unknown)
            return new FileClassification(filePath, formatByExtension);

        var response = await chatAgentInvoker.InvokeChatAsync(
            ClassifySystemPrompt,
            $"Classify this file: {filePath}",
            null,
            cancellationToken);

        return new FileClassification(filePath, ParseFormat(response.Trim()));
    }

    private FileFormat DetectByExtension(string filePath) =>
        Path.GetExtension(filePath).ToLowerInvariant() switch
        {
            ".csv" => FileFormat.Csv,
            ".log" or ".txt" => FileFormat.Log,
            ".jpg" or ".jpeg" or ".png" or ".tiff" or ".pdf" => FileFormat.Ocr,
            _ => FileFormat.Unknown
        };

    private FileFormat ParseFormat(string response) =>
        response.ToLowerInvariant() switch
        {
            "csv" => FileFormat.Csv,
            "log" => FileFormat.Log,
            "ocr" => FileFormat.Ocr,
            _ => FileFormat.Unknown
        };
}
