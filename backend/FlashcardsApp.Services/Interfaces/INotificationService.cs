using FlashcardsApp.Models.Notifications;

namespace FlashcardsApp.Services.Interfaces;

/// <summary>
/// Сервис для отправки real-time уведомлений пользователям
/// Абстракция позволяет легко заменить реализацию (SignalR, Push, Email и т.д.)
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Отправить уведомление о разблокировке достижения конкретному пользователю
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="notification">Данные уведомления</param>
    Task SendAchievementUnlockedAsync(Guid userId, AchievementUnlockedNotification notification);
    
    /// <summary>
    /// Отправить уведомление о разблокировке нескольких достижений
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="notifications">Список уведомлений</param>
    Task SendMultipleAchievementsUnlockedAsync(Guid userId, List<AchievementUnlockedNotification> notifications);
}
