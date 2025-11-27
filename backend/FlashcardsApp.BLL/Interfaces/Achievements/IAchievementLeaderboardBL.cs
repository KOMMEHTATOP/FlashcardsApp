using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.Models.DTOs.Achievements.Responses;

namespace FlashcardsApp.BLL.Interfaces.Achievements;

public interface IAchievementLeaderboardBL
{

    Task<ServiceResult<IEnumerable<LeaderboardEntryDto>>> GetTopUsersByAchievementsAsync(int count = 10);
    Task<ServiceResult<int>> GetUserLeaderboardPositionAsync(Guid userId);
    Task<ServiceResult<IEnumerable<LeaderboardEntryDto>>> GetLeaderboardWithUserAsync(Guid userId, int topCount = 10);
}
