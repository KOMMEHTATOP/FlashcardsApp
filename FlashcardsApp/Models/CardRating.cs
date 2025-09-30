namespace FlashcardsApp.Models;

public class CardRating
{
    public Guid Id { get; set; }
    public required int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    public required Guid UserId { get; set; } 
    public User? User { get; set; } 
    public required Guid CardId { get; set; } 
    public Card? Card { get; set; }
}
