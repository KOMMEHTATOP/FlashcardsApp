namespace FlashcardsApp.Models.DTOs.Achievements.Responses;

public class LeaderboardEntryDto
{
    public Guid UserId { get; set; }
    public required string Username { get; set; }
    public string? AvatarUrl { get; set; }             // Опциональный аватар
    public int AchievementCount { get; set; }          // Количество достижений
    public int TotalXP { get; set; }                   // Общий опыт пользователя
    public int Position { get; set; }                  // Позиция в рейтинге (1, 2, 3...)
    public bool IsCurrentUser { get; set; }            // Это текущий пользователь?

}
