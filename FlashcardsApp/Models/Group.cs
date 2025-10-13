using FlashcardsAppContracts.Constants;


namespace FlashcardsApp.Models;

public class Group
{
    public Guid Id {get; set;}
    public required Guid UserId  {get; set;} 
    public required string GroupName  {get; set;}

    public GroupColor GroupColor { get; set; } = GroupColor.Green; 
    public DateTime CreatedAt { get; set; }
    public int Order { get; set; } = 0;
    
    public User? User { get; set; }
    public List<Card>? Cards { get; set; } 
}
