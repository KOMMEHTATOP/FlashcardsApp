using FlashcardsApp.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using NLog;
using NLog.Web;

var logger = LogManager.Setup()
    .LoadConfigurationFromFile(Path.Combine(AppContext.BaseDirectory, "nlog.config.xml"))
    .GetCurrentClassLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);
    var services = builder.Services;

    // LOGGING
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    // ASP.NET CORE SERVICES
    services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            // Автоматическая валидация ModelState с единообразным форматом ошибок
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return new BadRequestObjectResult(new { errors });
            };
        });

    // INFRASTRUCTURE LAYER 
    services
        .AddDatabaseConfiguration(builder.Configuration)
        .AddIdentityConfiguration()
        .AddJwtAuthentication(builder.Configuration);

    // APPLICATION LAYER
    services
        .AddBusinessLogics()
        .AddHealthChecks()
        .AddNpgSql(
            connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
            name: "PostgreSQL Database",
            timeout: TimeSpan.FromSeconds(3),
            tags: new[] { "db", "sql", "postgres" })
        .Services  
        .AddConfigures(builder)
        .AddServices();

    // CROSS-CUTTING CONCERNS
    services
        .AddLocalizationConfiguration()
        .AddCorsConfiguration(builder.Configuration, builder.Environment);

    // OTHER
    services.AddSignalRConfiguration(builder.Environment);

    // DEVELOPMENT TOOLS
    if (builder.Environment.IsDevelopment())
    {
        services.AddSwaggerDocumentation();
    }

    // BUILD & CONFIGURE PIPELINE
    var app = builder.Build();

    // APPLY DATABASE MIGRATIONS
    app.ApplyDatabaseMigrations();

    app.ConfigureMiddleware(builder.Configuration, builder.Environment);

    if (app.Environment.IsDevelopment())
    {
        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/")
            {
                context.Response.Redirect("/swagger");
                return;
            }

            await next();
        });
    }

    app.Run();
}
catch (Exception exception)
{
    logger.Error(exception, "Приложение остановлено из-за исключения");
    throw;
}
finally
{
    LogManager.Shutdown();
}

// HELPERS
static bool IsRunningInDocker()
{
    return Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
}