using System.CommandLine;
using System.Text.Json;
using MisteryApp.Abstractions.Ingest;
using MisteryApp.Common.Ingest;

namespace MisteryApp.Cli.Commands;

public static class IngestCommand
{
    public static Command Build(IHost host)
    {
        var pathArg = new Argument<string>("path");
        pathArg.Description = "Folder or file to ingest";
        var dryRunOption = new Option<bool>("--dry-run");
        dryRunOption.Description = "Run full pipeline but skip DB write";
        var formatOption = new Option<IngestFormat>("--format");
        formatOption.Description = "Override auto-detected format (default: auto)";

        var cmd = new Command("ingest", "Process mixed-format files into a unified schema");
        cmd.Arguments.Add(pathArg);
        cmd.Options.Add(dryRunOption);
        cmd.Options.Add(formatOption);

        cmd.SetAction(async (ParseResult result, CancellationToken ct) =>
        {
            var path = result.GetValue(pathArg)!;
            var dryRun = result.GetValue(dryRunOption);
            var format = result.GetValue(formatOption);

            await using var scope = host.Services.CreateAsyncScope();
            var pipeline = scope.ServiceProvider.GetRequiredService<IIngestPipeline>();

            var summary = await pipeline.RunAsync(path, dryRun, format, ct);
            PrintSummary(summary, dryRun);
            return summary.RecordsRejected > 0 ? 1 : 0;
        });

        return cmd;
    }

    private static void PrintSummary(IngestRunSummary summary, bool dryRun)
    {
        var dryRunLabel = dryRun ? " (dry-run)" : "";
        Console.WriteLine($"Ingest complete{dryRunLabel}: {summary.FilesProcessed} file(s) processed.");
        Console.WriteLine($"  Inserted: {summary.RecordsInserted}  Healed: {summary.RecordsHealed}  Rejected: {summary.RecordsRejected}");

        foreach (var error in summary.Errors)
            Console.Error.WriteLine($"  error: {error}");
    }
}
