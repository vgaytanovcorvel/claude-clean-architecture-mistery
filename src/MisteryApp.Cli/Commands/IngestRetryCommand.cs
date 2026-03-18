using System.CommandLine;
using MisteryApp.Abstractions.Exceptions;
using MisteryApp.Abstractions.Ingest.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MisteryApp.Cli.Commands;

public static class IngestRetryCommand
{
    public static Command Build(IHost host)
    {
        var runIdArgument = new Argument<Guid>("run-id");
        runIdArgument.Description = "ID of the ingest run to retry";

        var dryRunOption = new Option<bool>("--dry-run");
        dryRunOption.Description = "Process files without persisting results";

        var parallelOption = new Option<int?>("--parallel");
        parallelOption.Description = "Maximum number of files to process concurrently (default: 4)";

        var command = new Command("retry", "Retry rejected files from a previous ingest run");
        command.Arguments.Add(runIdArgument);
        command.Options.Add(dryRunOption);
        command.Options.Add(parallelOption);

        command.SetAction(async (ParseResult result, CancellationToken ct) =>
        {
            var runId = result.GetValue(runIdArgument);
            var dryRun = result.GetValue(dryRunOption);
            var parallel = result.GetValue(parallelOption) ?? 4;

            var pipeline = host.Services.GetRequiredService<IIngestPipeline>();

            try
            {
                var run = await pipeline.RetryAsync(runId, dryRun, parallel, ct);
                Console.WriteLine($"Retry run {run.RunId}: {run.Status}.");
                return run.Status is MisteryApp.Common.Enums.IngestRunStatus.Failed ? 1 : 0;
            }
            catch (NotFoundException)
            {
                Console.Error.WriteLine($"error: ingest run not found (RunId: {runId}).");
                return 1;
            }
        });

        return command;
    }
}
