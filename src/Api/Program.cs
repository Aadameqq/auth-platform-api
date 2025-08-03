using System.Text.Json.Serialization;
using Api.Auth;
using Api.Configs;
using Api.Middlewares;
using Core;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureOptions();
builder.Services.ConfigureInfrastructureDependencies();
builder.Services.ConfigureCoreDependencies();
builder.Services.ConfigureCors();

builder
    .Services.AddControllers(options =>
    {
        options.ModelBinderProviders.Insert(0, new AuthorizedUserBinderProvider());
        options.ModelBinderProviders.Insert(1, new AccessManagerBinderProvider());
    })
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureOpenApi();

var app = builder.Build();

app.UseOpenApi();
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<DelayMiddleware>();
}

app.UseMiddleware<JwtMiddleware>();
app.MapControllers();

app.Run();
