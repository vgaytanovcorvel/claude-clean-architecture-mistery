using MisteryApp.Abstractions.Ingest.Interfaces;
using MisteryApp.Abstractions.Ingest.Models;
using MisteryApp.Common.Enums;

namespace MisteryApp.Implementation.Ingest;

public class IngestFileProcessor(
    IFileClassifier dispatcher,
    ISchemaMapper schemaMapper,
    IQualityCritic qualityCritic,
    IHealerAgent healerAgent) : IIngestFileProcessor
{
    public virtual async Task<FileProcessResult> ProcessFileAsync(
        string filePath,
        FileFormat hintFormat,
        CancellationToken cancellationToken)
    {
        var classification = hintFormat == FileFormat.Auto
            ? await dispatcher.ClassifyAsync(filePath, cancellationToken)
            : new FileClassification(filePath, hintFormat);

        var record = await schemaMapper.MapAsync(classification, cancellationToken);
        var qualityReport = await qualityCritic.ReviewAsync(record, filePath, cancellationToken);

        if (qualityReport.IsAcceptable)
            return new FileProcessResult(filePath, true, qualityReport.Record, null);

        var healResult = await healerAgent.HealAsync(qualityReport, cancellationToken);
        return healResult.IsSuccess
            ? new FileProcessResult(filePath, true, healResult.Value, null)
            : new FileProcessResult(filePath, false, null, healResult.Error);
    }
}
