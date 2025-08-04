using Core.Other;
using Core.Ports;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class DependenciesConfig
{
    public static void ConfigureCoreDependencies(this IServiceCollection services)
    {
        services.AddScoped<SessionCreator, SessionCreatorImpl>();
        services.AddScoped<ConfirmationProviderFactory, ConfirmationProviderFactoryImpl>();
        services.AddScoped<EmailConfirmationProvider, EmailConfirmationProviderImpl>();
        services.AddScoped<ConfirmationService, ConfirmationService>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependenciesConfig).Assembly);
        });
    }
}
