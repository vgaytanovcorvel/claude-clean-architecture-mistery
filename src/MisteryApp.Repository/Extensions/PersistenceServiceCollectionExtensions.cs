using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MisteryApp.Abstractions.Ingest;
using MisteryApp.Repository.Contexts;
using MisteryApp.Repository.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class PersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContextFactory<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
                    sqlOptions.CommandTimeout(30);
                }));

        services.AddScoped<IIngestedRecordRepository, IngestedRecordRepository>();

        return services;
    }
}
