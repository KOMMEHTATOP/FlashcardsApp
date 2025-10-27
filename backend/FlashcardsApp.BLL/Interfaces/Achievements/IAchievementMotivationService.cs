using FlashcardsApp.Models.Enums;

namespace FlashcardsApp.BLL.Interfaces.Achievements;

public interface IAchievementMotivationService
{
    /// <summary>
    /// Сгенерировать мотивационное сообщение для достижения
    /// </summary>
    /// <param name="conditionType">Тип условия достижения</param>
    /// <param name="remainingValue">Сколько осталось до выполнения</param>
    /// <param name="achievementName">Название достижения</param>
    /// <returns>Мотивационное сообщение</returns>
    string GenerateMotivationalMessage(
        AchievementConditionType conditionType, 
        int remainingValue, 
        string achievementName);
}
