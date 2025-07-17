using Microsoft.OpenApi.Models;

namespace Api.Configs;

public static class OpenApiConfig
{
    public static void ConfigureOpenApi(this IServiceCollection services)
    {
        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
        };

        var securityRequirement = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                Array.Empty<string>()
            },
        };

        var info = new OpenApiInfo
        {
            Title = "Survey App API",
            Description =
                "Api definition for development purposes"
                + "You can add ?delayMs= to url to slow down response time for testing",
        };

        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", securityScheme);
            options.AddSecurityRequirement(securityRequirement);
            options.SwaggerDoc("docs", info);
        });
    }

    public static void UseOpenApi(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return;
        }

        app.UseSwagger(c =>
        {
            c.RouteTemplate = "api-docs/{documentName}/swagger.json";
        });
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/api-docs/docs/swagger.json", "Docs");
            c.RoutePrefix = "api-docs";
        });
    }
}
