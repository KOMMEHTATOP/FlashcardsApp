using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.Models.DTOs;
using FlashcardsApp.Models.DTOs.Subscriptions.Responses;

namespace FlashcardsApp.BLL.Interfaces;

public interface ISubscriptionBL
{
    Task<ServiceResult<IEnumerable<PublicGroupDto>>> GetPublicGroupsAsync(
        Guid currentUserId,
        string? search = null,
        string sortBy = "date",
        int page = 1,
        int pageSize = 20,
        Guid? tagId = null);
    
    Task<ServiceResult<IEnumerable<SubscribedGroupDto>>> GetSubscribedGroupsAsync(Guid userId);
    Task<ServiceResult<bool>> SubscribeToGroupAsync(Guid groupId, Guid subscriberUserId);
    Task<ServiceResult<bool>> UnsubscribeFromGroupAsync(Guid groupId, Guid subscriberUserId);
    Task<ServiceResult<int>> GetAuthorRatingAsync(Guid authorUserId);
    Task<ServiceResult<bool>> IsSubscribedAsync(Guid groupId, Guid userId);
    Task<ServiceResult<IEnumerable<object>>> GetPublicGroupCardsAsync(Guid groupId);
    Task<ServiceResult<PublicGroupDto>> GetPublicGroupDetailsAsync(Guid groupId, Guid currentUserId);
    Task<ServiceResult<IEnumerable<TagDto>>> GetTagsAsync();
}
