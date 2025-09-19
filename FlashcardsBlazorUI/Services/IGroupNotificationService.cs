namespace FlashcardsBlazorUI.Services;

public interface IGroupNotificationService
{
    event Action<string> OnGroupsReordered;
    void NotifyGroupsReordered(string sourceContainer);
}

public class GroupNotificationService : IGroupNotificationService
{
    public event Action<string> OnGroupsReordered;
    
    public void NotifyGroupsReordered(string sourceContainer)
    {
        Console.WriteLine($"NotificationService: Уведомление из {sourceContainer}. Подписчиков: {OnGroupsReordered?.GetInvocationList()?.Length ?? 0}");
        OnGroupsReordered?.Invoke(sourceContainer);
    }
}