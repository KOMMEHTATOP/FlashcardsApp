using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.Models.DTOs.Achievements.Responses;

namespace FlashcardsApp.BLL.Interfaces.Achievements;

/// <summary>
/// Сервис для работы с таблицей лидеров по достижениям (будущая функциональность)
/// </summary>
public interface IAchievementLeaderboardBL
{
    /// <summary>
    /// Получить топ пользователей по количеству достижений
    /// </summary>
    Task<ServiceResult<IEnumerable<LeaderboardEntryDto>>> GetTopUsersByAchievementsAsync(int count = 10);

    /// <summary>
    /// Получить позицию пользователя в таблице лидеров
    /// </summary>
    Task<ServiceResult<int>> GetUserLeaderboardPositionAsync(Guid userId);

    /// <summary>
    /// Получить таблицу лидеров с позицией текущего пользователя
    /// Если пользователь не в топе, он будет добавлен в конец списка с флагом IsCurrentUser
    /// </summary>
    /// <param name="userId">ID текущего пользователя</param>
    /// <param name="topCount">Количество пользователей в топе</param>
    /// <returns>Топ пользователей + текущий пользователь (если не в топе)</returns>
    Task<ServiceResult<IEnumerable<LeaderboardEntryDto>>> GetLeaderboardWithUserAsync(Guid userId, int topCount = 10);
}
