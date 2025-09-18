namespace FlashcardsAppContracts.DTOs.Responses;

public class ResultCardRating
{
    public Guid CardId { get; set; }
    public Guid UserId { get; set; }
    public Guid RatingId { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
}
