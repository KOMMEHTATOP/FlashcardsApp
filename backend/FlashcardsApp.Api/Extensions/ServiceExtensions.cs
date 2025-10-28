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
        services.AddScoped<IUserBL, UserBL>();
        services.AddScoped<IAuthBL, AuthBL>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IGroupBL, GroupBL>();
        services.AddScoped<ICardBL, CardBL>();
        services.AddScoped<IStudySettingsBL, StudySettingsBL>();
        services.AddScoped<IStudySessionBL, StudySessionBL>();
        services.AddScoped<IUserStatisticsBL, UserStatisticsBL>();

        // Gamification
        services.AddScoped<IGamificationBL, GamificationBL>();
        services.AddScoped<IStudyBL, StudyBL>();

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
