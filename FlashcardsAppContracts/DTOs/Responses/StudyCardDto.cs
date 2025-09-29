namespace FlashcardsAppContracts.DTOs.Responses;

public class StudyCardDto
{
    public Guid CardId { get; set; }
    public required string Question { get; set; }
    public required string Answer { get; set; }
    public int LastRating { get; set; }
}