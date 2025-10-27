namespace FlashcardsApp.Models.DTOs.Cards.Responses;

public class ResultCardRatingDto
{
    public Guid CardId { get; set; }
    public Guid UserId { get; set; }
    public Guid RatingId { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
}
