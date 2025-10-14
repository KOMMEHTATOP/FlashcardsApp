using FlashcardsAppContracts.Enums;

namespace FlashcardsAppContracts.DTOs.Achievements.Responses;

/// <summary>
/// Рекомендация достижения (близкие к разблокировке)
/// </summary>
public class AchievementRecommendationDto
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
    /// URL иконки (эмодзи)
    /// </summary>
    public required string IconUrl { get; set; }
    
    /// <summary>
    /// Gradient для UI (Tailwind CSS классы)
    /// </summary>
    public required string Gradient { get; set; }
    
    /// <summary>
    /// Редкость достижения
    /// </summary>
    public AchievementRarity Rarity { get; set; }
    
    /// <summary>
    /// Процент выполнения (0-100)
    /// </summary>
    public int ProgressPercentage { get; set; }
    
    /// <summary>
    /// Сколько осталось до разблокировки (значение)
    /// </summary>
    public int RemainingValue { get; set; }
    
    /// <summary>
    /// Тип условия достижения (для понимания контекста)
    /// </summary>
    public AchievementConditionType ConditionType { get; set; }
    
    /// <summary>
    /// Мотивационное сообщение ("Изучите еще 3 карточки для получения «Первые шаги»!")
    /// </summary>
    public required string MotivationalMessage { get; set; }
    
    /// <summary>
    /// Примерная оценка дней до получения (0 если невозможно оценить)
    /// </summary>
    public int EstimatedDaysToComplete { get; set; }
}
