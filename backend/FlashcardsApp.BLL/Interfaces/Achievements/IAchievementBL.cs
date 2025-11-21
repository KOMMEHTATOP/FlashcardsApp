using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.Models.DTOs.Achievements.Responses;

namespace FlashcardsApp.BLL.Interfaces.Achievements;

public interface IAchievementBL
{
    Task<ServiceResult<IEnumerable<AchievementDto>>> GetAllAchievementsAsync();
    Task<ServiceResult<IEnumerable<UnlockedAchievementDto>>> GetUserAchievementsAsync(Guid userId);
    Task<ServiceResult<IEnumerable<AchievementWithStatusDto>>> GetAllAchievementsWithStatusAsync(Guid userId);
    Task<ServiceResult<UserAchievementDto>> UnlockAchievementAsync(Guid userId, Guid achievementId);
    Task<ServiceResult<List<AchievementDto>>> CheckAndUnlockAchievementsAsync(Guid userId);
}