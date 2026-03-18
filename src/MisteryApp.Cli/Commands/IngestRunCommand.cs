using System.CommandLine;
using MisteryApp.Abstractions.Exceptions;
using MisteryApp.Abstractions.Ingest.Interfaces;
using MisteryApp.Common.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MisteryApp.Cli.Commands;

public static class IngestRunCommand
{
    public static Command Build(IHost host)
    {
        var pathArgument = new Argument<string>("path");
        pathArgument.Description = "Path to the file or directory to ingest";

        var dryRunOption = new Option<bool>("--dry-run");
        dryRunOption.Description = "Process files without persisting results";

        var formatOption = new Option<FileFormat?>("--format");
        formatOption.Description = "Hint format for all files (default: Auto)";

        var parallelOption = new Option<int?>("--parallel");
        parallelOption.Description = "Maximum number of files to process concurrently (default: 4)";

        var command = new Command("run", "Ingest files from a file or directory");
        command.Arguments.Add(pathArgument);
        command.Options.Add(dryRunOption);
        command.Options.Add(formatOption);
        command.Options.Add(parallelOption);

        command.SetAction(async (ParseResult result, CancellationToken ct) =>
        {
            var path = result.GetValue(pathArgument)!;
            var dryRun = result.GetValue(dryRunOption);
            var format = result.GetValue(formatOption) ?? FileFormat.Auto;
            var parallel = result.GetValue(parallelOption) ?? 4;

            var pipeline = host.Services.GetRequiredService<IIngestPipeline>();

            try
            {
                var run = await pipeline.RunAsync(path, dryRun, format, parallel, ct);
                Console.WriteLine($"Run {run.RunId}: {run.Status}.");
                return run.Status is MisteryApp.Common.Enums.IngestRunStatus.Failed ? 1 : 0;
            }
            catch (NotFoundException ex)
            {
                Console.Error.WriteLine($"error: {ex.Message}");
                return 1;
            }
        });

        return command;
    }
}
