using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.BLL.Interfaces.Achievements;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.Enums;

namespace FlashcardsApp.BLL.Implementations.Achievements;

public class AchievementEstimationBL : IAchievementEstimationBL
{
    private readonly IGamificationBL _gamificationBl;

    public AchievementEstimationBL(IGamificationBL gamificationBl)
    {
        _gamificationBl = gamificationBl;
    }
    
    private const int DefaultCardsPerDay = 5;
    private const int DefaultCreatedCardsPerDay = 2;
    private const int DefaultXpPerDay = 100;
    private const int DefaultStudyHoursPerDay = 1;

    public int EstimateDaysToComplete(
        AchievementConditionType conditionType, 
        int remainingValue, 
        UserStatistics userStats)
    {
        if (remainingValue <= 0)
            return 0;

        return conditionType switch
        {
            AchievementConditionType.CurrentStreak => remainingValue,
            AchievementConditionType.BestStreak => remainingValue,
            AchievementConditionType.TotalCardsStudied => 
                EstimateByUserPace(
                    remainingValue, 
                    userStats.TotalCardsStudied, 
                    userStats.LastStudyDate, 
                    DefaultCardsPerDay),

            AchievementConditionType.TotalCardsCreated => 
                EstimateByUserPace(
                    remainingValue, 
                    userStats.TotalCardsCreated, 
                    userStats.LastStudyDate, 
                    DefaultCreatedCardsPerDay),

            AchievementConditionType.Level => 
                EstimateDaysForLevel(remainingValue, userStats),

            AchievementConditionType.TotalXP => 
                EstimateByUserPace(
                    remainingValue, 
                    userStats.TotalXP, 
                    userStats.LastStudyDate, 
                    DefaultXpPerDay),

            AchievementConditionType.TotalStudyTimeHours => 
                EstimateByUserPace(
                    remainingValue, 
                    (int)userStats.TotalStudyTime.TotalHours, 
                    userStats.LastStudyDate, 
                    DefaultStudyHoursPerDay),
            AchievementConditionType.PerfectRatingsStreak => 0,
            _ => 0
        };
    }

    private static int EstimateByUserPace(
        int remainingValue, 
        int currentTotal, 
        DateTime? lastActivity, 
        int defaultPacePerDay)
    {
        if (currentTotal < 5 || lastActivity == null)
        {
            return (int)Math.Ceiling((double)remainingValue / defaultPacePerDay);
        }

        var daysSinceLastActivity = (DateTime.UtcNow - lastActivity.Value).Days;
        if (daysSinceLastActivity > 30)
        {
            return (int)Math.Ceiling((double)remainingValue / defaultPacePerDay);
        }

        var estimatedDaysActive = Math.Max(1, daysSinceLastActivity);
        var userPacePerDay = (double)currentTotal / estimatedDaysActive;

        userPacePerDay = Math.Min(userPacePerDay, defaultPacePerDay * 3);

        if (userPacePerDay < 0.1)
        {
            return (int)Math.Ceiling((double)remainingValue / defaultPacePerDay);
        }

        return (int)Math.Ceiling(remainingValue / userPacePerDay);
    }

    private int EstimateDaysForLevel(int targetLevel, UserStatistics userStats)
    {
        var currentXP = userStats.TotalXP;
        var requiredXP = _gamificationBl.CalculateXPForLevel(targetLevel);
        var remainingXP = requiredXP - currentXP;

        if (remainingXP <= 0)
            return 0;

        return EstimateByUserPace(
            remainingXP, 
            currentXP, 
            userStats.LastStudyDate, 
            DefaultXpPerDay);
    }
}