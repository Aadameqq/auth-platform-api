using Core.Commands;
using Core.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class DependenciesConfig
{
    public static void ConfigureCoreDependencies(this IServiceCollection services)
    {
        services.AddScoped<ActivateAccountCommandHandler>();
        services.AddScoped<CreateAccountCommandHandler>();
        services.AddSingleton<GetTokenPayloadQueryHandler>();
        services.AddScoped<GetCurrentAccountQueryHandler>();
        services.AddScoped<LogInCommandHandler>();
        services.AddScoped<LogOutCommandHandler>();
        services.AddScoped<RefreshTokensCommandHandler>();
        services.AddScoped<InitializePasswordResetCommandHandler>();
        services.AddScoped<AssignRoleCommandHandler>();
        services.AddScoped<UnassignRoleCommandHandler>();
        services.AddScoped<ListRolesQueryHandler>();
        services.AddScoped<ResetPasswordCommandHandler>();
    }
}
