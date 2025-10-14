using System.ComponentModel.DataAnnotations;


namespace FlashcardsApp.Models;

public class Group
{
    public Guid Id {get; set;}
    public required Guid UserId  {get; set;} 
    [MaxLength(200)]
    public required string GroupName  {get; set;}
    [MaxLength(100)]
    public required string GroupColor { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public int Order { get; set; } = 0;
    
    public User? User { get; set; }
    public List<Card>? Cards { get; set; } 
}
