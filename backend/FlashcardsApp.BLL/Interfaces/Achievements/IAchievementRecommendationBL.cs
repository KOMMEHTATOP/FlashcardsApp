using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.Models.DTOs.Achievements.Responses;

namespace FlashcardsApp.BLL.Interfaces.Achievements;

public interface IAchievementRecommendationBL
{
    /// <summary>
    /// Получить рекомендации: какие достижения пользователь скоро получит
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="count">Количество рекомендаций</param>
    /// <returns>Список рекомендуемых достижений</returns>
    Task<ServiceResult<IEnumerable<AchievementRecommendationDto>>> GetAchievementRecommendationsAsync(
        Guid userId, 
        int count = 3);
}
