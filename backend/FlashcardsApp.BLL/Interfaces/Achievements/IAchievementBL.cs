using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.Models.DTOs.Achievements.Responses;

namespace FlashcardsApp.BLL.Interfaces.Achievements;

/// <summary>
/// Сервис для управления достижениями пользователей
/// Отвечает за получение, разблокировку и проверку условий достижений
/// </summary>
public interface IAchievementBL
{
    /// <summary>
    /// Получить список всех доступных достижений в системе
    /// </summary>
    /// <returns>Список всех достижений</returns>
    Task<ServiceResult<IEnumerable<AchievementDto>>> GetAllAchievementsAsync();

    /// <summary>
    /// Получить список разблокированных достижений пользователя
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <returns>Список разблокированных достижений</returns>
    Task<ServiceResult<IEnumerable<UnlockedAchievementDto>>> GetUserAchievementsAsync(Guid userId);

    /// <summary>
    /// Получить все достижения со статусом разблокировки для конкретного пользователя
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <returns>Список всех достижений с флагом IsUnlocked</returns>
    Task<ServiceResult<IEnumerable<AchievementWithStatusDto>>> GetAllAchievementsWithStatusAsync(Guid userId);

    /// <summary>
    /// Разблокировать конкретное достижение для пользователя вручную
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="achievementId">ID достижения для разблокировки</param>
    /// <returns>Данные разблокированного достижения или ошибка</returns>
    Task<ServiceResult<UserAchievementDto>> UnlockAchievementAsync(Guid userId, Guid achievementId);

    /// <summary>
    /// Автоматически проверить статистику пользователя и разблокировать достижения
    /// Вызывается после завершения сессии обучения или обновления статистики
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <returns>Список новых разблокированных достижений</returns>
    Task<ServiceResult<List<AchievementDto>>> CheckAndUnlockAchievementsAsync(Guid userId);
}