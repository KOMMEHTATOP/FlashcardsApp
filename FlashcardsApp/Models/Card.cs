namespace FlashcardsApp.Models;

public class Card
{
    public Guid Id { get; set; }
    public required string Question  { get; set; }
    public required string Answer { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public Guid UserId { get; set; }  // Foreign key
    public User? User { get; set; } // Navigation property
    public required Guid GroupId { get; set; } //Foreign key
    public Group? Group { get; set; } // Navigation Property - передаем сам объект
    public List<CardRating>? Ratings { get; set; } //Navigation property
}
