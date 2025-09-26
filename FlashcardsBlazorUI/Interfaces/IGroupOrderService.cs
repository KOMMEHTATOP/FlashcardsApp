using FlashcardsAppContracts.DTOs.Requests;

namespace FlashcardsBlazorUI.Interfaces;

public interface IGroupOrderService
{
    event Action OnGroupsReordered;
    Task<bool> ReorderGroupsAsync(List<ReorderGroupDto> reorderList);
    void NotifyGroupsReordered();
}
