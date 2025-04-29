using Infrastructure.Options;

namespace Api.Configs;

public static class OptionsConfig
{
    public static void ConfigureOptions(this WebApplicationBuilder builder)
    {
        var customEnv = Environment.GetEnvironmentVariable("CUSTOM_APPSETTINGS");
        if (!string.IsNullOrEmpty(customEnv))
        {
            builder.Configuration.AddJsonFile($"appsettings.{customEnv}.json", false, false);
        }

        builder.Configuration.AddEnvironmentVariables();
        builder.Configuration.AddUserSecrets<Program>();

        var services = builder.Services;
        AddOptions<AuthOptions>(services, "Auth");
        AddOptions<OAuthOptions>(services, "OAuth");
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
