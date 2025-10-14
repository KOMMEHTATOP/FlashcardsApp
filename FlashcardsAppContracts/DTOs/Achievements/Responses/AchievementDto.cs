namespace FlashcardsAppContracts.DTOs.Achievements.Responses;

public class AchievementDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string IconUrl { get; set; }
    public required string Gradient { get; set; }  
}
