namespace FlashcardsApp.DAL.Models;

public class Group
{
    public Guid Id {get; set;}
    public required Guid UserId  {get; set;} 
    public required string GroupName  {get; set;}
    public required string GroupColor { get; set; } = "";
    public required string GroupIcon {get; set;} = "";
    public DateTime CreatedAt { get; set; }
    public int Order { get; set; }
    
    
    public bool IsPublished { get; set; }
    public int StarRating { get; set; }
    
    public User? User { get; set; }
    public List<Card>? Cards { get; set; } = [];
}
