using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.Models.DTOs.Groups.Requests;
using FlashcardsApp.Models.DTOs.Groups.Responses;

namespace FlashcardsApp.BLL.Interfaces;

public interface IGroupBL
{
    Task<ServiceResult<ResultGroupDto>> GetGroupByIdAsync(Guid groupId, Guid userId);
    Task<ServiceResult<IEnumerable<ResultGroupDto>>> GetGroupsAsync(Guid userId);
    Task<ServiceResult<ResultGroupDto>> CreateGroupAsync(CreateGroupDto model, Guid userId);
    Task<ServiceResult<ResultGroupDto>> UpdateGroupAsync(Guid groupId, Guid userId, CreateGroupDto model);
    Task<ServiceResult<bool>> DeleteGroupAsync(Guid groupId, Guid userId);
    Task<ServiceResult<bool>> UpdateGroupsOrderAsync(List<ReorderGroupDto> groupOrders, Guid userId);
    Task<ServiceResult<bool>> ChangeAccessGroupAsync(Guid groupId, Guid userId, bool isPublish);
}