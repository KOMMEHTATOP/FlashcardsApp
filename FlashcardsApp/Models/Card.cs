namespace FlashcardsApp.Models;

public class Card
{
    public Guid CardId { get; set; }
    public required string Question  { get; set; }
    public required string Answer { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public Guid UserId { get; set; }  
    public User? User { get; set; } 
    public required Guid GroupId { get; set; } 
    public Group? Group { get; set; } 
    public List<CardRating>? Ratings { get; set; } 
}
