using FlashcardsApp.Models;
using FlashcardsAppContracts.Enums;

namespace FlashcardsApp.Interfaces.Achievements;

public interface IAchievementEstimationService
{
    /// <summary>
    /// Оценить количество дней до выполнения достижения
    /// </summary>
    /// <param name="conditionType">Тип условия достижения</param>
    /// <param name="remainingValue">Сколько осталось до выполнения</param>
    /// <param name="userStats">Статистика пользователя</param>
    /// <returns>Примерное количество дней (0 если невозможно оценить)</returns>
    int EstimateDaysToComplete(
        AchievementConditionType conditionType, 
        int remainingValue, 
        UserStatistics userStats);

}
