using MisteryApp.Abstractions.Ingest.Models;
using MisteryApp.Common.Enums;

namespace MisteryApp.Abstractions.Ingest.Interfaces;

public interface IIngestFileProcessor
{
    /// <summary>
    /// Processes a single file through the full ingest pipeline: classify, map, review, and optionally heal.
    /// </summary>
    /// <param name="filePath">Absolute path to the file to process.</param>
    /// <param name="hintFormat">
    /// A format hint that can short-circuit agent classification. Pass <see cref="FileFormat.Auto"/>
    /// to let the agent classify the file format automatically based on its extension and content.
    /// Pass any other <see cref="FileFormat"/> value to skip classification and use that format directly.
    /// </param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task<FileProcessResult> ProcessFileAsync(string filePath, FileFormat hintFormat, CancellationToken cancellationToken);
}
