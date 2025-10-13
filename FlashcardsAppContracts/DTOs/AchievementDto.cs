namespace FlashcardsAppContracts.DTOs;

public class UserAchievementDto
{
    public Guid UserId { get; set; }
    public Guid AchievementId { get; set; }
    public DateTime UnlockedAt { get; set; }
    public AchievementDto Achievement { get; set; }
}

public class AchievementDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconUrl { get; set; }
    public string Gradient { get; set; }  
}
