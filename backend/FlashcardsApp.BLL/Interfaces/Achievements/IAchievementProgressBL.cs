using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.Models.DTOs.Achievements.Responses;

namespace FlashcardsApp.BLL.Interfaces.Achievements;

public interface IAchievementProgressBL
{
    Task<ServiceResult<AchievementProgressDto>> CalculateAchievementProgressAsync(Guid userId, Guid achievementId);
    Task<ServiceResult<IEnumerable<AchievementProgressDto>>> GetAllAchievementsProgressAsync(Guid userId);
}
