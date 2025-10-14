namespace FlashcardsAppContracts.DTOs.Achievements.Responses;

public class AchievementRecommendationDto
{
    public Guid AchievementId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string IconUrl { get; set; }
    public required string Gradient { get; set; }
    public int ProgressPercentage { get; set; }                 // Процент выполнения
    public required string MotivationalMessage { get; set; }    // "Осталось 2 дня до streak!"
    public int EstimatedDaysToComplete { get; set; }            // Примерная оценка дней до получения

}
