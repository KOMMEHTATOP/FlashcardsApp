using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.Enums;

namespace FlashcardsApp.BLL.Interfaces.Achievements;

public interface IAchievementEstimationBL
{
    int EstimateDaysToComplete(AchievementConditionType conditionType, int remainingValue, UserStatistics userStats);

}
