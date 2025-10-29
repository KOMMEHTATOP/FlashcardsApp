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
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserBL, UserBL>();
        services.AddScoped<IAuthBL, AuthBL>();
        services.AddScoped<IGroupBL, GroupBL>();
        services.AddScoped<ICardBL, CardBL>();
        
        //Обучение
        services.AddScoped<IStudyBL, StudyBL>();
        services.AddScoped<IStudySessionBL, StudySessionBL>();
        services.AddScoped<IStudySettingsBL, StudySettingsBL>();
        
        // Statistic
        services.AddScoped<IUserStatisticsBL, UserStatisticsBL>();
        
        // Gamification
        services.AddScoped<IGamificationBL, GamificationBL>();

        // Notifications
        services.AddScoped<INotificationService, SignalRNotificationService>();

        // Achievements
        services.AddScoped<IAchievementBL, AchievementBL>();
        services.AddScoped<IAchievementRewardBL, AchievementRewardBL>();
        services.AddScoped<IAchievementLeaderboardBL, AchievementLeaderboardBL>();
        services.AddScoped<IAchievementProgressBL, AchievementProgressBL>();
        services.AddScoped<IAchievementMotivationBL, AchievementMotivationBL>();
        services.AddScoped<IAchievementEstimationBL, AchievementEstimationBL>();
        services.AddScoped<IAchievementRecommendationBL, AchievementRecommendationBL>();

        return services;
    }
}
