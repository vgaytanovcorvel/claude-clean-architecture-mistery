using System.CommandLine;
using MisteryApp.Abstractions.Ingest.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MisteryApp.Cli.Commands;

public static class IngestListCommand
{
    public static Command Build(IHost host)
    {
        var limitOption = new Option<int?>("--limit");
        limitOption.Description = "Maximum number of runs to display (default: 20)";

        var command = new Command("list", "List recent ingest runs");
        command.Options.Add(limitOption);

        command.SetAction(async (ParseResult result, CancellationToken ct) =>
        {
            var limit = result.GetValue(limitOption) ?? 20;
            var pipeline = host.Services.GetRequiredService<IIngestPipeline>();
            var summaries = await pipeline.GetRunSummariesAsync(limit, ct);

            if (summaries.Count == 0)
            {
                Console.WriteLine("No ingest runs found.");
                return 0;
            }

            Console.WriteLine($"{"RunId",-36}  {"Status",-14}  {"Started",-16}  {"Total",5}  {"OK",5}  {"Rejected",8}  Path");
            Console.WriteLine(new string('-', 108));
            foreach (var s in summaries)
            {
                var ok = s.ProcessedFiles - s.RejectedFiles;
                Console.WriteLine($"{s.RunId,-36}  {s.Status,-14}  {s.StartedAt:yyyy-MM-dd HH:mm}  {s.TotalFiles,5}  {ok,5}  {s.RejectedFiles,8}  {s.InputPath}");
            }

            return 0;
        });

        return command;
    }
}
