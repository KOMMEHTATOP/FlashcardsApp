using FlashcardsApp.DAL;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace FlashcardsApp.Api.Extensions;

public static class DatabaseExtensions
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    
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
    
    public static IApplicationBuilder ApplyDatabaseMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        try
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
            if (context.Database.CanConnect())
            {
                context.Database.Migrate();
            }
            else
            {
                throw new InvalidOperationException("Database connection failed");
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "✗ Ошибка при применении миграций");
            throw; 
        }

        return app;
    }
}