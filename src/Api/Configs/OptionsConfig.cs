using Infrastructure.Options;

namespace Api.Configs;

public static class OptionsConfig
{
    public static void ConfigureOptions(this IServiceCollection services)
    {
        AddOptions<AuthOptions>(services, "Auth");
        AddOptions<DatabaseOptions>(services, "Database");
        AddOptions<SmtpOptions>(services, "Smtp");
        AddOptions<RedisOptions>(services, "Redis");
        AddOptions<AccountOptions>(services, "Account");
    }

    private static void AddOptions<T>(IServiceCollection services, string sectionName)
        where T : class
    {
        services
            .AddOptions<T>()
            .BindConfiguration(sectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}
