namespace FlashcardsAppContracts.DTOs.Responses;

public class StudyRewardDto
{
    public int XPEarned { get; set; }
    public int TotalXP { get; set; }
    public int CurrentLevel { get; set; }
    public bool LeveledUp { get; set; }
    public int? NewLevel { get; set; }
    public bool StreakIncreased { get; set; }
    public int CurrentStreak { get; set; }
}
