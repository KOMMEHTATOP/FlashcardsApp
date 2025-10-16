using FlashcardsApp.Services;
using FlashcardsAppContracts.DTOs.Auth.Responses;

namespace FlashcardsApp.Interfaces;

public interface IUserService
{
    Task<ServiceResult<UserDashboardDto>> GetUserDashboardAsync(Guid userId);

}
