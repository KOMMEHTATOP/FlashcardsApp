using FlashcardsApp.Services;
using FlashcardsAppContracts.DTOs.Achievements.Responses;

namespace FlashcardsApp.Interfaces.Achievements;

/// <summary>
/// Сервис для работы с таблицей лидеров по достижениям (будущая функциональность)
/// </summary>
public interface IAchievementLeaderboardService
{
    /// <summary>
    /// Получить топ пользователей по количеству достижений
    /// </summary>
    /// <param name="count">Количество пользователей в топе</param>
    /// <returns>Список лидеров</returns>
    Task<ServiceResult<IEnumerable<LeaderboardEntryDto>>> GetTopUsersByAchievementsAsync(int count = 10);

    /// <summary>
    /// Получить позицию пользователя в таблице лидеров
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <returns>Позиция в рейтинге</returns>
    Task<ServiceResult<int>> GetUserLeaderboardPositionAsync(Guid userId);
}