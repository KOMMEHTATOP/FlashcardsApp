using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.Enums;

namespace FlashcardsApp.BLL.Interfaces.Achievements;

public interface IAchievementEstimationBL
{
    /// <summary>
    /// Оценить количество дней до выполнения достижения
    /// </summary>
    /// <param name="conditionType">Тип условия достижения</param>
    /// <param name="remainingValue">Сколько осталось до выполнения</param>
    /// <param name="userStats">Статистика пользователя</param>
    /// <returns>Примерное количество дней (0 если невозможно оценить)</returns>
    int EstimateDaysToComplete(AchievementConditionType conditionType, int remainingValue, UserStatistics userStats);

}
