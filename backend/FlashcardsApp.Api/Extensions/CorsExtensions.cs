namespace FlashcardsApp.Api.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment,
        ILogger logger)
    {
        var allowedOrigins = configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        if (allowedOrigins.Length == 0 && environment.IsDevelopment())
        {
            logger.LogWarning("⚠️ No CORS origins configured! Using defaults.");
            allowedOrigins = 
            [
                "http://localhost:3000",
                "http://localhost:5173",
                "http://localhost:7255",
                "https://localhost:7255"
            ];
        }

        if (allowedOrigins.Length > 0)
        {
            logger.LogInformation("🌐 CORS enabled for: {Origins}", 
                string.Join(", ", allowedOrigins));

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policyBuilder =>
                {
                    policyBuilder.WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }
        else if (environment.IsProduction())
        {
            logger.LogCritical("❌ No CORS origins configured for Production!");
            throw new Exception("CORS origins must be configured for Production.");
        }

        return services;
    }
}
