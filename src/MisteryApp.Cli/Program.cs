using MisteryApp.Cli.Commands;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((ctx, config) =>
    {
        config.SetBasePath(AppContext.BaseDirectory)
              .AddJsonFile("appsettings.json", optional: true);
        config.AddUserSecrets(System.Reflection.Assembly.GetExecutingAssembly(), optional: true);
        config.AddEnvironmentVariables(prefix: "MISTERYAPP_");
    })
    .ConfigureServices((ctx, services) =>
    {
        services.AddMisteryAppServices();
        services.AddPersistence(ctx.Configuration);
    })
    .Build();

var rootCommand = new RootCommand("MisteryApp CLI");

var ingestCommand = new Command("ingest", "Ingest unstructured files into structured records");
ingestCommand.Subcommands.Add(IngestRunCommand.Build(host));
ingestCommand.Subcommands.Add(IngestListCommand.Build(host));
ingestCommand.Subcommands.Add(IngestRetryCommand.Build(host));
rootCommand.Subcommands.Add(ingestCommand);

return await rootCommand.Parse(args).InvokeAsync();
