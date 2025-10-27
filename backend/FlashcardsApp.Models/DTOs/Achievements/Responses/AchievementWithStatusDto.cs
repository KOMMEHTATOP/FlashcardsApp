using FlashcardsApp.Models.Enums;

namespace FlashcardsApp.Models.DTOs.Achievements.Responses;

public class AchievementWithStatusDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string IconUrl { get; set; }
    public string? Gradient { get; set; }
    
    /// <summary>
    /// Тип условия для разблокировки
    /// </summary>
    public AchievementConditionType ConditionType { get; set; }
    
    /// <summary>
    /// Значение условия
    /// </summary>
    public int ConditionValue { get; set; }
    
    /// <summary>
    /// Редкость достижения
    /// </summary>
    public AchievementRarity Rarity { get; set; }
    
    /// <summary>
    /// Разблокировано ли достижение пользователем
    /// </summary>
    public bool IsUnlocked { get; set; }
}
