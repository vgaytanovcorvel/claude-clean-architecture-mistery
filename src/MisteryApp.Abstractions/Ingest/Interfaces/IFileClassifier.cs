using MisteryApp.Abstractions.Ingest.Models;

namespace MisteryApp.Abstractions.Ingest.Interfaces;

public interface IFileClassifier
{
    Task<FileClassification> ClassifyAsync(string filePath, CancellationToken cancellationToken);
}
