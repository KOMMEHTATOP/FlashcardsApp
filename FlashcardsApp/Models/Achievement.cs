namespace FlashcardsApp.Models;

public class Achievement
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string IconUrl { get; set; }
    public List<UserAchievement>? UserAchievements { get; set; }
}
