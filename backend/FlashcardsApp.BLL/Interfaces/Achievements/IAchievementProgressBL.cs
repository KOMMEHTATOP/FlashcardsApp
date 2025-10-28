using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.Models.DTOs.Achievements.Responses;

namespace FlashcardsApp.BLL.Interfaces.Achievements;

/// <summary>
/// Сервис для вычисления прогресса достижений
/// Отвечает ТОЛЬКО за расчет прогресса (текущее значение, требуемое значение, процент)
/// </summary>
public interface IAchievementProgressBL
{
    /// <summary>
    /// Вычислить процент выполнения достижения
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="achievementId">ID достижения</param>
    /// <returns>Прогресс выполнения достижения</returns>
    Task<ServiceResult<AchievementProgressDto>> CalculateAchievementProgressAsync(Guid userId, Guid achievementId);

    /// <summary>
    /// Получить прогресс по всем достижениям
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <returns>Список прогресса всех достижений</returns>
    Task<ServiceResult<IEnumerable<AchievementProgressDto>>> GetAllAchievementsProgressAsync(Guid userId);
}
