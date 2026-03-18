namespace MisteryApp.Common.Ingest;

public record HealedRecord(MappedRecord Record, IReadOnlyList<string> HealingNotes);
