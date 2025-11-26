namespace FlashcardsApp.Models.DTOs.Leaderboard;

public class LeaderboardDto
{
    public List<LeaderboardEntryDto> TopList { get; set; } 
    public int TotalUsersCount { get; set; }
}
