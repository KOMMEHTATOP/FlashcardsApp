namespace FlashcardsAppContracts.DTOs.Achievements.Responses;


public class UserAchievementDto
{
    public Guid UserId { get; set; }
    public Guid AchievementId { get; set; }
    public DateTime UnlockedAt { get; set; }
    public AchievementDto Achievement { get; set; }
}
