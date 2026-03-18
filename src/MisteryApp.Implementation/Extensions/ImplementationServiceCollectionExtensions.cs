using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using MisteryApp.Abstractions.Ingest;
using MisteryApp.Implementation.Ingest;

namespace Microsoft.Extensions.DependencyInjection;

public static class ImplementationServiceCollectionExtensions
{
    public static IServiceCollection AddMisteryAppServices(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddValidatorsFromAssembly(typeof(ImplementationServiceCollectionExtensions).Assembly);

        services.AddSingleton(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var apiKey = config["OpenAI:ApiKey"]
                ?? throw new InvalidOperationException(
                    "OpenAI API key is not configured. Set the MISTERYAPP_OPENAI__APIKEY environment variable.");
            var model = config["OpenAI:Model"] ?? "gpt-4o-mini";

            return Kernel.CreateBuilder()
                .AddOpenAIChatCompletion(model, apiKey)
                .Build();
        });

        services.AddSingleton<IKernelInvoker, KernelInvoker>();
        services.AddScoped<IIngestDispatcher, IngestDispatcher>();
        services.AddScoped<ISchemaMapper, SchemaMapper>();
        services.AddScoped<IQualityCritic, QualityCritic>();
        services.AddScoped<IHealerAgent, HealerAgent>();
        services.AddScoped<IIngestPipeline, IngestPipeline>();

        return services;
    }
}
