using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.Models.DTOs.Subscriptions.Responses;

namespace FlashcardsApp.BLL.Interfaces;

/// <summary>
/// Сервис для управления подписками на группы
/// </summary>
public interface ISubscriptionBL
{
    /// <summary>
    /// Получить список публичных групп для подписки
    /// </summary>
    Task<ServiceResult<IEnumerable<PublicGroupDto>>> GetPublicGroupsAsync(string? search = null);
    
    /// <summary>
    /// Получить группы, на которые подписан пользователь
    /// </summary>
    Task<ServiceResult<IEnumerable<SubscribedGroupDto>>> GetSubscribedGroupsAsync(Guid userId);
    
    /// <summary>
    /// Подписаться на группу
    /// </summary>
    Task<ServiceResult<bool>> SubscribeToGroupAsync(Guid groupId, Guid subscriberUserId);
    
    /// <summary>
    /// Отписаться от группы
    /// </summary>
    Task<ServiceResult<bool>> UnsubscribeFromGroupAsync(Guid groupId, Guid subscriberUserId);
    
    /// <summary>
    /// Получить рейтинг автора (сумма подписчиков всех его групп)
    /// </summary>
    Task<ServiceResult<int>> GetAuthorRatingAsync(Guid authorUserId);
    
    /// <summary>
    /// Проверить, подписан ли пользователь на группу
    /// </summary>
    Task<ServiceResult<bool>> IsSubscribedAsync(Guid groupId, Guid userId);
}
