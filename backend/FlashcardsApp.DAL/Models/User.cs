using Microsoft.AspNetCore.Identity;

namespace FlashcardsApp.DAL.Models;

public class User : IdentityUser<Guid>
{
    public required string Role { get; set; }
    public string? Login { get; set; }
    public List<Card>? Cards { get; set; }
    public List<StudyHistory>? StudyHistory { get; set; }
    public List<Group>? Groups { get; set; }
    public UserStatistics? Statistics { get; set; }
    public List<UserAchievement>? UserAchievements { get; set; }
}
