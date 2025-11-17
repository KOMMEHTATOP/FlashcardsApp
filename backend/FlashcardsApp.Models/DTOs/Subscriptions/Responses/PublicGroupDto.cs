namespace FlashcardsApp.Models.DTOs.Subscriptions.Responses;

public class PublicGroupDto
{
    public Guid Id { get; set; }
    public required string GroupName { get; set; }
    public required string GroupColor { get; set; }
    public required string GroupIcon { get; set; }
    public required string AuthorName { get; set; } 
    public int CardCount { get; set; }
    public int SubscriberCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsSubscribed { get; set; }
}
