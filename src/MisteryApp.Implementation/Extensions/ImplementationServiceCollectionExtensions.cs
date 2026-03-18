using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using MisteryApp.Abstractions.Ingest.Interfaces;
using MisteryApp.Implementation.Ingest;
using MisteryApp.Implementation.Ingest.Agents;
using MisteryApp.Implementation.Ingest.Plugins;

namespace Microsoft.Extensions.DependencyInjection;

public static class ImplementationServiceCollectionExtensions
{
    public static IServiceCollection AddMisteryAppServices(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddValidatorsFromAssembly(typeof(ImplementationServiceCollectionExtensions).Assembly);

        services.AddSingleton<Kernel>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var apiKey = config["OpenAI:ApiKey"]
                ?? throw new InvalidOperationException("OpenAI:ApiKey not configured. Set MISTERYAPP_OPENAI__APIKEY environment variable.");
            var model = config["OpenAI:Model"] ?? "gpt-4o-mini";

            var kernel = Kernel.CreateBuilder()
                .AddOpenAIChatCompletion(model, apiKey)
                .Build();
            kernel.Plugins.AddFromType<FileIoPlugin>();
            return kernel;
        });

        services.AddScoped<IChatAgentInvoker, ChatAgentInvoker>();
        services.AddScoped<IFileClassifier, IngestDispatcher>();
        services.AddScoped<ISchemaMapper, SchemaMapper>();
        services.AddScoped<IQualityCritic, QualityCritic>();
        services.AddScoped<IHealerAgent, HealerAgent>();
        services.AddScoped<IIngestFileProcessor, IngestFileProcessor>();
        services.AddScoped<IIngestPipeline, IngestPipeline>();

        return services;
    }
}
