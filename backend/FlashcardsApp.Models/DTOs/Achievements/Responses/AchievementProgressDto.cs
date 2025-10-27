using FlashcardsApp.Models.Enums;

namespace FlashcardsApp.Models.DTOs.Achievements.Responses;

/// <summary>
/// Прогресс выполнения достижения
/// </summary>
public class AchievementProgressDto
{
    /// <summary>
    /// ID достижения
    /// </summary>
    public Guid AchievementId { get; set; }
    
    /// <summary>
    /// Название достижения
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Описание достижения
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// URL иконки (эмодзи)
    /// </summary>
    public string IconUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Gradient для UI (Tailwind CSS классы)
    /// </summary>
    public string Gradient { get; set; } = string.Empty;
    
    /// <summary>
    /// Тип условия достижения
    /// </summary>
    public AchievementConditionType ConditionType { get; set; }
    
    /// <summary>
    /// Редкость достижения
    /// </summary>
    public AchievementRarity Rarity { get; set; }
    
    /// <summary>
    /// Требуемое значение для разблокировки
    /// </summary>
    public int ConditionValue { get; set; }
    
    /// <summary>
    /// Текущее значение пользователя
    /// </summary>
    public int CurrentValue { get; set; }
    
    /// <summary>
    /// Процент выполнения (0-100)
    /// </summary>
    public int ProgressPercentage { get; set; }
    
    /// <summary>
    /// Достижение разблокировано?
    /// </summary>
    public bool IsUnlocked { get; set; }
    
    /// <summary>
    /// Дата разблокировки (если разблокировано)
    /// </summary>
    public DateTime? UnlockedAt { get; set; }
}
