namespace FlashcardsApp.Models.DTOs.Achievements.Responses;

public class AchievementRewardDto
{
    public Guid AchievementId { get; set; }
    public required string AchievementName { get; set; }
    public int XPAwarded { get; set; }                 // Начисленный опыт
    public int? CoinsAwarded { get; set; }             // Начисленные монеты (если будут)
    public DateTime AwardedAt { get; set; }            // Когда начислена награда
}
