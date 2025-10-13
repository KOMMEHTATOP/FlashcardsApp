namespace FlashcardsAppContracts.DTOs.Responses;

public class UserStatsDto
{
    public int TotalXP { get; set; }
    public int Level { get; set; }
    public int XPForNextLevel { get; set; }
    
    public int XPProgressInCurrentLevel { get; set; }  // Сколько набрано в текущем уровне
    public int XPRequiredForCurrentLevel { get; set; }  // Всего нужно для текущего уровня
    
    public int CurrentStreak { get; set; }
    public int BestStreak { get; set; }
    public TimeSpan TotalStudyTime { get; set; }
}
