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
            var endpoint = config["AzureOpenAI:Endpoint"]
                ?? throw new InvalidOperationException("AzureOpenAI:Endpoint not configured. Set MISTERYAPP_AZUREOPENAI__ENDPOINT environment variable.");
            var apiKey = config["AzureOpenAI:ApiKey"]
                ?? throw new InvalidOperationException("AzureOpenAI:ApiKey not configured. Set MISTERYAPP_AZUREOPENAI__APIKEY environment variable.");
            var deploymentName = config["AzureOpenAI:DeploymentName"] ?? "gpt-4o-mini";

            var kernel = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey)
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
