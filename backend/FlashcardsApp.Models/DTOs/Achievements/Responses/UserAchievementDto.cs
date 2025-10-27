namespace FlashcardsApp.Models.DTOs.Achievements.Responses;

public class UserAchievementDto
{
    public Guid AchievementId { get; set; }
    public required string AchievementName { get; set; }
    public DateTime UnlockedAt { get; set; }
}
