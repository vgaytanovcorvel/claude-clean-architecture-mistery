using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace MisteryApp.Implementation.Ingest.Plugins;

public class FileIoPlugin
{
    [KernelFunction]
    [Description("Read the entire content of a file from an absolute path")]
    public virtual async Task<string> ReadFileAsync(
        [Description("Absolute path to the file")] string filePath,
        CancellationToken cancellationToken = default)
    {
        ValidatePath(filePath);
        return await File.ReadAllTextAsync(filePath, cancellationToken);
    }

    [KernelFunction]
    [Description("List all entries in a directory at an absolute path")]
    public virtual string ListDirectory(
        [Description("Absolute path to the directory")] string directoryPath)
    {
        ValidatePath(directoryPath);
        var entries = Directory.GetFileSystemEntries(directoryPath);
        return string.Join("\n", entries);
    }

    [KernelFunction]
    [Description("Get metadata for a file at an absolute path")]
    public virtual string GetFileMetadata(
        [Description("Absolute path to the file")] string filePath)
    {
        ValidatePath(filePath);
        if (!File.Exists(filePath))
            return "File not found.";
        var info = new FileInfo(filePath);
        return $"Name: {info.Name}, Size: {info.Length} bytes, LastModified: {info.LastWriteTimeUtc:O}, Extension: {info.Extension}";
    }

    private static void ValidatePath(string path)
    {
        if (!Path.IsPathRooted(path))
            throw new UnauthorizedAccessException($"Only absolute paths are allowed (Path: {path}).");
        var resolved = Path.GetFullPath(path);
        var allowedRoot = Path.GetPathRoot(resolved) ?? resolved;
        if (!resolved.StartsWith(allowedRoot, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException($"Access denied: path is outside the allowed directory (Path: {path}).");
    }
}
