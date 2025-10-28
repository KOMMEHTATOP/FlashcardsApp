using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.Models.DTOs.Statistics.Responses;

namespace FlashcardsApp.BLL.Interfaces;

public interface IUserStatisticsBL
{
    Task<ServiceResult<UserStatsDto>> GetUserStatsAsync(Guid userId);
    
}
