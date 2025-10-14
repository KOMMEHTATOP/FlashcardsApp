namespace FlashcardsApp.Models.Notifications;

/// <summary>
/// Модель уведомления о разблокировке достижения
/// Отправляется клиенту через SignalR
/// </summary>
public class AchievementUnlockedNotification
{
    /// <summary>
    /// ID достижения
    /// </summary>
    public Guid AchievementId { get; set; }
    
    /// <summary>
    /// Название достижения
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Описание достижения
    /// </summary>
    public required string Description { get; set; }
    
    /// <summary>
    /// URL иконки достижения
    /// </summary>
    public required string IconUrl { get; set; }
    
    /// <summary>
    /// Градиент для отображения (опционально)
    /// </summary>
    public string? Gradient { get; set; }
    
    /// <summary>
    /// Редкость достижения
    /// </summary>
    public string Rarity { get; set; } = string.Empty;
    
    /// <summary>
    /// Время разблокировки
    /// </summary>
    public DateTime UnlockedAt { get; set; }
    
    /// <summary>
    /// Бонусные очки опыта, полученные за достижение
    /// </summary>
    public int BonusXP { get; set; }
}
