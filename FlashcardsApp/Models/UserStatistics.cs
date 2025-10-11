namespace FlashcardsApp.Models;

public class UserStatistics
{
    public Guid UserId { get; set; }  // PK и FK
    
    public int TotalXP { get; set; }
    public int CurrentStreak { get; set; }
    public int BestStreak { get; set; }
    public DateTime LastStudyDate { get; set; }
    public TimeSpan TotalStudyTime { get; set; }
    
    public User User { get; set; } 
}
