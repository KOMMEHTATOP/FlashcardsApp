using FlashcardsApp.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using NLog;
using NLog.Web;

// Настройка NLog перед стартом билдера
var logger = LogManager.Setup()
    .LoadConfigurationFromFile(Path.Combine(AppContext.BaseDirectory, "nlog.config.xml"))
    .GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    var services = builder.Services;

    // --- ИСПРАВЛЕНИЕ ЛОГИРОВАНИЯ ---
    builder.Logging.ClearProviders(); // Удаляем дефолтные провайдеры (Debug, EventSource и т.д.)
    builder.Logging.AddConsole();     // <--- ВЕРНУЛИ КОНСОЛЬ! Теперь мы увидим "Now listening on..."
    builder.Host.UseNLog();           // Подключаем NLog для всего остального
    // -------------------------------

    services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return new BadRequestObjectResult(new { errors });
            };
        });

    services
        .AddDatabaseConfiguration(builder.Configuration)
        .AddIdentityConfiguration()
        .AddJwtAuthentication(builder.Configuration);

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

    services
        .AddLocalizationConfiguration()
        .AddCorsConfiguration(builder.Configuration, builder.Environment);

    services.AddSignalRConfiguration(builder.Environment);

    if (builder.Environment.IsDevelopment())
    {
        services.AddSwaggerDocumentation();
    }

    var app = builder.Build();

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
    // NLog перехватит критическую ошибку старта (если будет)
    logger.Error(exception, "Приложение остановлено из-за исключения");
    throw;
}
finally
{
    // Обязательно сбрасываем логгер при выходе
    LogManager.Shutdown();
}

static bool IsRunningInDocker()
{
    return Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
}