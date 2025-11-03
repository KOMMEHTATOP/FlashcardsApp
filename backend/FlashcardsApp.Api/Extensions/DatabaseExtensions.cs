using FlashcardsApp.DAL;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace FlashcardsApp.Api.Extensions;

public static class DatabaseExtensions
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Настройка подключения к базе данных PostgreSQL
    /// </summary>
    public static IServiceCollection AddDatabaseConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new Exception("Connection string is not configured.");
        }

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }

    /// <summary>
    /// Применение миграций базы данных при запуске приложения
    /// Fail-fast: если миграции не применятся, приложение не запустится
    /// </summary>
    public static IApplicationBuilder ApplyDatabaseMigrations(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            try
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                Logger.Info("Проверка подключения к базе данных...");
                
                // Проверка что БД доступна
                if (context.Database.CanConnect())
                {
                    Logger.Info("Применение миграций базы данных...");
                    context.Database.Migrate();
                    Logger.Info("✓ Миграции успешно применены");
                }
                else
                {
                    Logger.Error("✗ Не удалось подключиться к базе данных");
                    throw new InvalidOperationException("Database connection failed");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "✗ Ошибка при применении миграций");
                throw; // Fail-fast principle
            }
        }

        return app;
    }
}