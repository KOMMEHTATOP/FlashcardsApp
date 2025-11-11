namespace FlashcardsApp.Models.DTOs.Cards.Responses;

public class ResultCardDto
{
    public required Guid CardId { get; set; }
    public required Guid GroupId { get; set; }
    public required string Question { get; set; }
    public required string Answer { get; set; }
    
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int LastRating { get; set; }

}
