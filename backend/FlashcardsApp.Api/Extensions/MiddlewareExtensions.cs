using FlashcardsApp.Api.Hubs;
using FlashcardsApp.DAL;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.DAL.Seeds;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace FlashcardsApp.Api.Extensions;

public static class MiddlewareExtensions
{
    public static WebApplication ConfigureMiddleware(
        this WebApplication app,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("MiddlewareConfiguration");

        // Локализация
        var localizationOptions = app.Services
            .GetRequiredService<IOptions<RequestLocalizationOptions>>();
        app.UseRequestLocalization(localizationOptions.Value);

        // HTTPS только в Production
        if (environment.IsProduction())
        {
            app.UseHttpsRedirection();
        }

        // Middleware
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();

        // Swagger только не в Production
        if (!environment.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Seed initial data (migrations are applied in DatabaseExtensions)
        var autoSeed = configuration.GetValue("AutoSeed", false);

        if (autoSeed)
        {
            app.ApplySeedData(logger);
        }
        else
        {
            logger.LogInformation("Auto seed disabled");
        }

        logger.LogInformation("Application started in {Environment} mode",
            environment.EnvironmentName);

        // Контроллеры
        app.MapControllers();

        // Health Check endpoint с проверкой БД
        app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var result = System.Text.Json.JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        duration = e.Value.Duration.TotalMilliseconds,
                        exception = e.Value.Exception?.Message
                    }),
                    totalDuration = report.TotalDuration.TotalMilliseconds
                });
                await context.Response.WriteAsync(result);
            }
        });
        
        // SignalR Hub
        app.MapHub<NotificationHub>("/hubs/notifications");

        return app;
    }

    /// <summary>
    /// Заполнение базы данных начальными данными (seed)
    /// Миграции должны быть применены ДО вызова этого метода
    /// </summary>
    private static void ApplySeedData(this WebApplication app, ILogger logger)
    {
        logger.LogInformation("=== AUTO SEED ENABLED ===");

        using var scope = app.Services.CreateScope();

        try
        {
            logger.LogInformation("Starting database seeding...");

            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            SeedManager.SeedAsync(db, userManager).GetAwaiter().GetResult();

            logger.LogInformation("✓ Database seeding completed successfully!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "✗ Error during database seeding");
            // Не падаем - seed не критичен для запуска приложения
        }
    }
}
