namespace FlashcardsApp.Models.DTOs.Subscriptions.Responses;

public class SubscribedGroupDto
{
    public Guid Id { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string GroupColor { get; set; } = string.Empty;
    public string GroupIcon { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public int CardCount { get; set; }
    public DateTime SubscribedAt { get; set; }

}
