using FlashcardsApp.Api.Infrastructure.Notifications;
using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.BLL.Implementations.Achievements;
using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.BLL.Interfaces.Achievements;
using FlashcardsApp.Services.Implementations;
using FlashcardsApp.Services.Interfaces;

namespace FlashcardsApp.Api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Core services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<ICardService, CardService>();
        services.AddScoped<IStudySettingsService, StudySettingsService>();
        services.AddScoped<IStudySessionService, StudySessionService>();
        services.AddScoped<IUserStatisticsService, UserStatisticsService>();

        // Gamification
        services.AddScoped<IGamificationService, GamificationService>();
        services.AddScoped<IStudyService, StudyService>();

        // Notifications
        services.AddScoped<INotificationService, SignalRNotificationService>();

        // Achievements
        services.AddScoped<IAchievementService, AchievementService>();
        services.AddScoped<IAchievementRewardService, AchievementRewardService>();
        services.AddScoped<IAchievementLeaderboardService, AchievementLeaderboardService>();
        services.AddScoped<IAchievementProgressService, AchievementProgressService>();
        services.AddScoped<IAchievementMotivationService, AchievementMotivationService>();
        services.AddScoped<IAchievementEstimationService, AchievementEstimationService>();
        services.AddScoped<IAchievementRecommendationService, AchievementRecommendationService>();

        return services;
    }
}
