using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.BLL.Implementations.Achievements;
using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.BLL.Interfaces.Achievements;

namespace FlashcardsApp.Api.Extensions;

public static class BusinessExtensions
{
    public static IServiceCollection AddBusinessLogics(
        this IServiceCollection services)
    {
        // Core services
        services.AddScoped<IUserBL, UserBL>();
        services.AddScoped<IAuthBL, AuthBL>();
        services.AddScoped<IGroupBL, GroupBL>();
        services.AddScoped<ICardBL, CardBL>();
        
        //Study
        services.AddScoped<IStudyBL, StudyBL>();
        services.AddScoped<IStudySessionBL, StudySessionBL>();
        services.AddScoped<IStudySettingsBL, StudySettingsBL>();
        
        // Statistic
        services.AddScoped<IUserStatisticsBL, UserStatisticsBL>();
        
        // Gamification
        services.AddScoped<IGamificationBL, GamificationBL>();

        // Achievements
        services.AddScoped<IAchievementBL, AchievementBL>();
        services.AddScoped<IAchievementRewardBL, AchievementRewardBL>();
        services.AddScoped<IAchievementLeaderboardBL, AchievementLeaderboardBL>();
        services.AddScoped<IAchievementProgressBL, AchievementProgressBL>();
        services.AddScoped<IAchievementMotivationBL, AchievementMotivationBL>();
        services.AddScoped<IAchievementEstimationBL, AchievementEstimationBL>();
        services.AddScoped<IAchievementRecommendationBL, AchievementRecommendationBL>();
        
        //Создание групп и карточек
        services.AddScoped<IImportBL, ImportBL>();
        
        //Подписки на общие группы
        services.AddScoped<ISubscriptionBL, SubscriptionBL>();
        
        // Лидерборд
        services.AddScoped<ILeaderboardBL, LeaderboardBL>();

        return services;
    }
}
