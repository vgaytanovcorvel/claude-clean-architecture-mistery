namespace MisteryApp.Abstractions.Ingest.Models;

public record IngestRejectedFile(Guid Id, Guid FileId, string RejectionReason);
