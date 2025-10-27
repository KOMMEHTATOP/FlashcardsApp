using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.Models.DTOs.Auth.Responses;

namespace FlashcardsApp.BLL.Interfaces;

public interface IUserService
{
    Task<ServiceResult<UserDashboardDto>> GetUserDashboardAsync(Guid userId);

}
