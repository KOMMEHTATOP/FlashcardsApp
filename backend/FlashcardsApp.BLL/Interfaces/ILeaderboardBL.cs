using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.Models.DTOs.Leaderboard;

namespace FlashcardsApp.BLL.Interfaces;

public interface ILeaderboardBL
{
    Task<ServiceResult<LeaderboardDto>> GetLeaderboardAsync(Guid userId);
}
