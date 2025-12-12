using FlashcardsApp.BLL.Hubs;
using FlashcardsApp.BLL.Interfaces.Achievements;
using FlashcardsApp.DAL;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.DAL.Seeds;
using Microsoft.AspNetCore.Hosting.Server; 
using Microsoft.AspNetCore.Hosting.Server.Features; 
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

        var localizationOptions = app.Services
            .GetRequiredService<IOptions<RequestLocalizationOptions>>();
        app.UseRequestLocalization(localizationOptions.Value);

        if (environment.IsProduction())
        {
            app.UseHttpsRedirection();
        }

        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();

        if (!environment.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

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
        
        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStarted.Register(() =>
        {
            var server = app.Services.GetRequiredService<IServer>();
            var addressFeature = server.Features.Get<IServerAddressesFeature>();

            if (addressFeature != null)
            {
                foreach (var address in addressFeature.Addresses)
                {
                    logger.LogInformation("Now listening on: {Address}", address);
                }
            }
        });

        app.MapControllers();

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
        
        app.MapHub<NotificationHub>("/api/notificationHub");

        return app;
    }

    private static void ApplySeedData(this WebApplication app, ILogger logger)
    {
        logger.LogDebug("=== AUTO SEED ENABLED ===");

        using var scope = app.Services.CreateScope();

        try
        {
            logger.LogDebug("Starting database seeding...");

            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            SeedManager.SeedAsync(db, userManager, logger).GetAwaiter().GetResult();

            logger.LogDebug("✓ Database seeding completed!");

            var achievementBL = scope.ServiceProvider.GetRequiredService<IAchievementBL>();
            var testUserId = SeedManager.GetTestUserId();

            var result = achievementBL.CheckAndUnlockAchievementsAsync(testUserId)
                .GetAwaiter().GetResult();

            if (result is { IsSuccess: true, Data.Count: > 0 })
            {
                logger.LogDebug("✓ {Count} achievements unlocked for test user", result.Data.Count);
            }
            else
            {
                logger.LogDebug("✓ No new achievements to unlock");
            }

            logger.LogInformation("✓ Seed process completed successfully!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "✗ Error during database seeding");
        }
    }
}