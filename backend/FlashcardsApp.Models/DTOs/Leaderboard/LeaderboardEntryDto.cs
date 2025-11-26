namespace FlashcardsApp.Models.DTOs.Leaderboard;

public class LeaderboardEntryDto
{
    public int Position { get; set; }
    public Guid UserId { get; set; }
    public required string Login { get; set; }
    public int TotalRating { get; set; }
}
