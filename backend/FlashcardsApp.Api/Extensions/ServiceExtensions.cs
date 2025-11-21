using FlashcardsApp.Api.Infrastructure.Notifications;
using FlashcardsApp.Services.Implementations;
using FlashcardsApp.Services.Interfaces;

namespace FlashcardsApp.Api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<INotificationService, SignalRNotificationService>();
        
        return services;
    }
}
