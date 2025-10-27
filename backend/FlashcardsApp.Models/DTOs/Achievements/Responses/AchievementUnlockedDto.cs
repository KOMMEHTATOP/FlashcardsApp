using FlashcardsApp.Models.Enums;

namespace FlashcardsApp.Models.DTOs.Achievements.Responses;

/// <summary>
/// DTO для уведомления о разблокированном достижении
/// Используется когда пользователь получает новое достижение
/// </summary>
public class AchievementUnlockedDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string IconUrl { get; set; }
    
    /// <summary>
    /// Редкость достижения
    /// </summary>
    public AchievementRarity Rarity { get; set; }
}
