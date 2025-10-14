using FlashcardsApp.Services;
using FlashcardsAppContracts.DTOs.Achievements.Responses;

namespace FlashcardsApp.Interfaces.Achievements;

/// <summary>
/// Сервис для вычисления прогресса достижений (будущая функциональность)
/// </summary>
public interface IAchievementProgressService
{
    /// <summary>
    /// Вычислить процент выполнения достижения
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="achievementId">ID достижения</param>
    /// <returns>Процент выполнения (0-100)</returns>
    Task<ServiceResult<AchievementProgressDto>> CalculateAchievementProgressAsync(Guid userId, Guid achievementId);

    /// <summary>
    /// Получить рекомендации: какие достижения пользователь скоро получит
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="count">Количество рекомендаций</param>
    /// <returns>Список рекомендуемых достижений</returns>
    Task<ServiceResult<IEnumerable<AchievementRecommendationDto>>> GetAchievementRecommendationsAsync(Guid userId, int count = 3);
}