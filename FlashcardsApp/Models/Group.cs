using FlashcardsApp.Constants;

namespace FlashcardsApp.Models;

public class Group
{
    public Guid Id {get; set;}
    public required Guid UserId  {get; set;} //Foreign key
    public required string GroupName  {get; set;}
    
    public string GroupColor { get; set; } = GroupColors.Default;
    public DateTime CreatedAt { get; set; }
    
    public User? User { get; set; }
    public List<Card>? Cards { get; set; } //Navigation property
}
