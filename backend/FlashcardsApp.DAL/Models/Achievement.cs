using FlashcardsApp.Models.Enums;

namespace FlashcardsApp.DAL.Models;

public class Achievement
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
    /// Значение условия (например, 10 карточек, 7 дней streak)
    /// </summary>
    public int ConditionValue { get; set; }
    
    /// <summary>
    /// Редкость достижения
    /// </summary>
    public AchievementRarity Rarity { get; set; }
    
    /// <summary>
    /// Порядок отображения
    /// </summary>
    public int DisplayOrder { get; set; }
    
    /// <summary>
    /// Активно ли достижение
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    public List<UserAchievement>? UserAchievements { get; set; }
}
