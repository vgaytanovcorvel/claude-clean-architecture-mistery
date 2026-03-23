using FluentValidation;
using MisteryApp.Abstractions.Interfaces;
using MisteryApp.Implementation.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ImplementationServiceCollectionExtensions
{
    public static IServiceCollection AddMisteryAppServices(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddValidatorsFromAssembly(typeof(ImplementationServiceCollectionExtensions).Assembly);

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITodoService, TodoService>();

        return services;
    }
}
