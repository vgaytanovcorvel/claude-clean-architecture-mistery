using MisteryApp.Common.Enums;

namespace MisteryApp.Abstractions.Ingest.Models;

public record FileClassification(string FilePath, FileFormat Format);
