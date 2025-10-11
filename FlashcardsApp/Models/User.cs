using Microsoft.AspNetCore.Identity;

namespace FlashcardsApp.Models;

public class User : IdentityUser<Guid>
{
    public List<Card>? Cards { get; set; }
    public List<CardRating>? CardRatings { get; set; }
    public List<Group>? Groups { get; set; }

    public int TotalXP { get; set; }
    public int Level { get; set; }
    public int CurrentStreak { get; set; }
    public int BestStreak { get; set; }
    public DateTime LastStudyDate { get; set; }
    public TimeSpan? TotalStudyTime { get; set; }
    public TimeSpan? CurrentStreakTime { get; set; }
    public TimeSpan? BestStreakTime { get; set; }
    public List<UserAchievement>? Achievements { get; set; }
}
