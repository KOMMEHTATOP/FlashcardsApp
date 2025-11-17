using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.Models.DTOs.Subscriptions.Responses;

namespace FlashcardsApp.BLL.Interfaces;

public interface ISubscriptionBL
{
    Task<ServiceResult<IEnumerable<PublicGroupDto>>> GetPublicGroupsAsync(
        Guid currentUserId,
        string? search = null,
        string sortBy = "date",
        int page = 1,
        int pageSize = 20);
    
    Task<ServiceResult<IEnumerable<SubscribedGroupDto>>> GetSubscribedGroupsAsync(Guid userId);
    
    Task<ServiceResult<bool>> SubscribeToGroupAsync(Guid groupId, Guid subscriberUserId);
    
    Task<ServiceResult<bool>> UnsubscribeFromGroupAsync(Guid groupId, Guid subscriberUserId);
    
    Task<ServiceResult<int>> GetAuthorRatingAsync(Guid authorUserId);
    
    Task<ServiceResult<bool>> IsSubscribedAsync(Guid groupId, Guid userId);
    
    /// <summary>
    /// Получить карточки публичной группы для предпросмотра перед подпиской
    /// </summary>
    Task<ServiceResult<IEnumerable<object>>> GetPublicGroupCardsAsync(Guid groupId);
    
    /// <summary>
    /// Получить детали публичной группы по ID, включая статус подписки текущего пользователя.
    /// </summary>
    // Тип возвращаемого значения изменен на ваш PublicGroupDto:
    Task<ServiceResult<PublicGroupDto>> GetPublicGroupDetailsAsync(Guid groupId, Guid currentUserId);
}
