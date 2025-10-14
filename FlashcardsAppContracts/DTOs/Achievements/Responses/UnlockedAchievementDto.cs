using FlashcardsAppContracts.Enums;

namespace FlashcardsAppContracts.DTOs.Achievements.Responses;

public class UnlockedAchievementDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string IconUrl { get; set; }
    public string? Gradient { get; set; }
    
    /// <summary>
    /// Редкость достижения
    /// </summary>
    public AchievementRarity Rarity { get; set; }
    
    /// <summary>
    /// Дата и время разблокировки
    /// </summary>
    public DateTime UnlockedAt { get; set; }
}
