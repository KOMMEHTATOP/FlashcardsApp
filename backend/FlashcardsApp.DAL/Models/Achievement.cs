using FlashcardsApp.Models.Enums;

namespace FlashcardsApp.DAL.Models;

public class Achievement
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string IconUrl { get; set; }
    public string? Gradient { get; set; }
    public AchievementConditionType ConditionType { get; set; }
    public int ConditionValue { get; set; }
    public AchievementRarity Rarity { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public List<UserAchievement>? UserAchievements { get; set; }
}
