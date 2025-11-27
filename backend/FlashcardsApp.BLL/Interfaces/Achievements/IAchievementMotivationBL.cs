using FlashcardsApp.Models.Enums;

namespace FlashcardsApp.BLL.Interfaces.Achievements;

public interface IAchievementMotivationBL
{
    string GenerateMotivationalMessage(
        AchievementConditionType conditionType, 
        int remainingValue, 
        string achievementName);
}
