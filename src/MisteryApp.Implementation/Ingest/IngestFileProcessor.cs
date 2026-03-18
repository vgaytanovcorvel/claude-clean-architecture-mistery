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

        var records = await schemaMapper.MapAsync(classification, cancellationToken);
        if (records.Count == 0)
            return new FileProcessResult(filePath, false, null, "No records extracted from file.");

        var accepted = await ReviewAndHealRecordsAsync(records, filePath, cancellationToken);
        return accepted.Count > 0
            ? new FileProcessResult(filePath, true, accepted, null)
            : new FileProcessResult(filePath, false, null, "All extracted records were rejected.");
    }

    private async Task<IReadOnlyList<MappedRecord>> ReviewAndHealRecordsAsync(
        IReadOnlyList<MappedRecord> records,
        string filePath,
        CancellationToken cancellationToken)
    {
        var accepted = new List<MappedRecord>();
        foreach (var record in records)
        {
            var acceptedRecord = await ReviewAndHealSingleAsync(record, filePath, cancellationToken);
            if (acceptedRecord is not null)
                accepted.Add(acceptedRecord);
        }
        return accepted;
    }

    private async Task<MappedRecord?> ReviewAndHealSingleAsync(
        MappedRecord record,
        string filePath,
        CancellationToken cancellationToken)
    {
        var qualityReport = await qualityCritic.ReviewAsync(record, filePath, cancellationToken);
        if (qualityReport.IsAcceptable)
            return qualityReport.Record;

        var healResult = await healerAgent.HealAsync(qualityReport, cancellationToken);
        return healResult.IsSuccess ? healResult.Value : null;
    }
}
