using FlashcardsApp.Services;
using FlashcardsAppContracts.DTOs.Responses;
using FlashcardsAppContracts.DTOs.Statistics.Responses;

namespace FlashcardsApp.Interfaces;

public interface IUserStatisticsService
{
    Task<ServiceResult<UserStatsDto>> GetUserStatsAsync(Guid userId);
    
}
