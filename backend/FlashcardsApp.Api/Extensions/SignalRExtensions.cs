namespace FlashcardsApp.Api.Extensions;

public static class SignalRExtensions
{
    public static IServiceCollection AddSignalRConfiguration(
        this IServiceCollection services,
        IWebHostEnvironment environment)
    {
        services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = environment.IsDevelopment();
            options.KeepAliveInterval = TimeSpan.FromSeconds(15);
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}
