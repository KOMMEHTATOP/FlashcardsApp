namespace FlashcardsApp.Models;

public class CardRating
{
    public Guid Id { get; set; }
    public required Guid UserId { get; set; } //Foreign key
    public required int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public required Guid CardId { get; set; } //Foreign key
    public Card? Card { get; set; } //Navigation property
}
