using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.Models.DTOs.Groups.Requests;
using FlashcardsApp.Models.DTOs.Groups.Responses;

namespace FlashcardsApp.BLL.Interfaces;

/// <summary>
/// Сервис для управления группами карточек
/// </summary>
public interface IGroupBL
{
    /// <summary>
    /// Получить группу по ID с проверкой прав доступа
    /// </summary>
    /// <param name="groupId">ID группы</param>
    /// <param name="userId">ID пользователя</param>
    /// <returns>Результат с данными группы или ошибкой</returns>
    Task<ServiceResult<ResultGroupDto>> GetGroupByIdAsync(Guid groupId, Guid userId);

    /// <summary>
    /// Получить все группы пользователя с сортировкой
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <returns>Список групп пользователя</returns>
    Task<ServiceResult<IEnumerable<ResultGroupDto>>> GetGroupsAsync(Guid userId);

    /// <summary>
    /// Создать новую группу с валидацией уникальности имени
    /// </summary>
    /// <param name="model">Данные новой группы</param>
    /// <param name="userId">ID владельца группы</param>
    /// <returns>Результат с созданной группой или ошибкой</returns>
    Task<ServiceResult<ResultGroupDto>> CreateGroupAsync(CreateGroupDto model, Guid userId);

    /// <summary>
    /// Обновить группу с валидацией прав доступа
    /// </summary>
    /// <param name="groupId">ID группы</param>
    /// <param name="userId">ID пользователя</param>
    /// <param name="model">Новые данные группы</param>
    /// <returns>Результат с обновленной группой или ошибкой</returns>
    Task<ServiceResult<ResultGroupDto>> UpdateGroupAsync(Guid groupId, Guid userId, CreateGroupDto model);

    /// <summary>
    /// Удалить группу с проверкой прав доступа
    /// </summary>
    /// <param name="groupId">ID группы</param>
    /// <param name="userId">ID пользователя</param>
    /// <returns>Результат операции</returns>
    Task<ServiceResult<bool>> DeleteGroupAsync(Guid groupId, Guid userId);

    /// <summary>
    /// Обновить порядок отображения групп
    /// </summary>
    /// <param name="groupOrders">Список групп с новым порядком</param>
    /// <param name="userId">ID пользователя</param>
    /// <returns>Результат операции</returns>
    Task<ServiceResult<bool>> UpdateGroupsOrderAsync(List<ReorderGroupDto> groupOrders, Guid userId);
    
    /// <summary>
    /// Открыть, закрыть общий доступ к группе
    /// </summary>
    /// <param name="groupId">ID группы</param>
    /// <param name="isPublish">доступ к группе</param>
    /// <returns>Результат операции</returns>
    Task<ServiceResult<bool>> ChangeAccessGroupAsync(Guid groupId, Guid userId, bool isPublish);
}