namespace FlashcardsApp.DAL.Models;

public class UserGroupSubscription
{
    public Guid Id { get; set; }
    
    // Кто подписался
    public Guid SubscriberUserId { get; set; }
    public User? SubscriberUser { get; set; }
    
    // На какую группу подписался
    public Guid GroupId { get; set; }
    public Group? Group { get; set; }
    
    public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;
}
