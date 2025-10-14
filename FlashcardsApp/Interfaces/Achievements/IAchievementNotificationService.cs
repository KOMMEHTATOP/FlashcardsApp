using FlashcardsApp.Services;

namespace FlashcardsApp.Interfaces.Achievements;

/// <summary>
/// Сервис для отправки уведомлений о достижениях (будущая функциональность)
/// </summary>
public interface IAchievementNotificationService
{
    /// <summary>
    /// Отправить уведомление о разблокировке достижения
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="achievementId">ID достижения</param>
    Task<ServiceResult<bool>> SendAchievementUnlockedNotificationAsync(Guid userId, Guid achievementId);

    /// <summary>
    /// Отправить напоминание о почти выполненном достижении
    /// </summary>
    Task<ServiceResult<bool>> SendAchievementReminderAsync(Guid userId, Guid achievementId);
}