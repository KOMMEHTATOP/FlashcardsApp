namespace FlashcardsBlazorUI.Interfaces;

public interface IGroupNotificationService
{
    event Action<string> OnGroupsReordered;
    void NotifyGroupsReordered(string sourceContainer);
}
