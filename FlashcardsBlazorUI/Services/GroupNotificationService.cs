using FlashcardsBlazorUI.Interfaces;

namespace FlashcardsBlazorUI.Services;

public class GroupNotificationService : IGroupNotificationService
{
    public event Action<string> OnGroupsReordered;
    
    public void NotifyGroupsReordered(string sourceContainer)
    {
        Console.WriteLine($"NotificationService: Уведомление из {sourceContainer}. Подписчиков: {OnGroupsReordered?.GetInvocationList()?.Length ?? 0}");
        OnGroupsReordered?.Invoke(sourceContainer);
    }
}