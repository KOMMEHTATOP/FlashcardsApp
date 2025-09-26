using FlashcardsAppContracts.DTOs.Requests;
using FlashcardsBlazorUI.Interfaces;

namespace FlashcardsBlazorUI.Services;

public class GroupOrderService : IGroupOrderService
{
    private readonly GroupService _groupService;
    public event Action OnGroupsReordered;

    public GroupOrderService(GroupService groupService)
    {
        _groupService = groupService;
    }

    public async Task<bool> ReorderGroupsAsync(List<ReorderGroupDto> reorderList)
    {
        var result = await _groupService.UpdateGroupOrderAsync(reorderList);
        if (result)
        {
            NotifyGroupsReordered();
        }
        return result;
    }

    public void NotifyGroupsReordered() 
    {
        Console.WriteLine($"GroupOrderService: NotifyGroupsReordered вызван. Подписчиков: {OnGroupsReordered?.GetInvocationList()?.Length ?? 0}");
        OnGroupsReordered?.Invoke();
    }
    
}