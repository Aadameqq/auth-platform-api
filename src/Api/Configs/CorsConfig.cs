namespace Api.Configs;

public static class CorsConfig
{
    public static void ConfigureCors(this IServiceCollection services, IWebHostEnvironment env)
    {
        if (!env.IsDevelopment())
        {
            return;
        }

        services.AddCors(options =>
        {
            options.AddPolicy(
                "Development",
                policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                }
            );
        });
    }

    public static void UseCors(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (!env.IsDevelopment())
        {
            return;
        }

        app.UseCors("Development");
    }
}
