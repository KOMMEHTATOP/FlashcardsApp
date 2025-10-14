namespace FlashcardsAppContracts.DTOs.Achievements.Responses;

public class AchievementProgressDto
{
    public Guid AchievementId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public int CurrentProgress { get; set; }     // Текущее значение (например, 5 дней streak)
    public int RequiredProgress { get; set; }    // Требуемое значение (например, 7 дней)
    public int ProgressPercentage { get; set; }  // Процент выполнения (71%)
    public bool IsUnlocked { get; set; }         // Уже разблокировано или нет

}
