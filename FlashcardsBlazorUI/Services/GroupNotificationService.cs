using FlashcardsBlazorUI.Interfaces;

namespace FlashcardsBlazorUI.Services;

public class GroupNotificationService : IGroupNotificationService
{
    public event Action<string>? OnGroupsReordered;
    
    public void NotifyGroupsReordered(string sourceContainer)
    {
        OnGroupsReordered?.Invoke(sourceContainer);
    }
}