using FlashcardsApp.Api.Hubs;
using FlashcardsApp.DAL;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.DAL.Seeds;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        // HTTPS только не в Production
        if (!environment.IsProduction())
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

        // Auto-migration
        var autoMigrate = configuration.GetValue("AutoMigrate", false);
        if (autoMigrate)
        {
            app.ApplyMigrationsAndSeed(logger);
        }
        else
        {
            logger.LogInformation("Auto migration disabled");
        }

        logger.LogInformation("Application started in {Environment} mode", 
            environment.EnvironmentName);

        // Контроллеры
        app.MapControllers();

        // SignalR Hub
        app.MapHub<NotificationHub>("/hubs/notifications");

        return app;
    }

    private static void ApplyMigrationsAndSeed(this WebApplication app, ILogger logger)
    {
        logger.LogInformation("=== AUTO MIGRATION ENABLED ===");

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        logger.LogInformation("Running migrations...");
        db.Database.Migrate();
        logger.LogInformation("Migrations completed!");

        logger.LogInformation("Starting seed...");
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        SeedManager.SeedAsync(db, userManager).GetAwaiter().GetResult();
        logger.LogInformation("Seed completed!");
    }
}
