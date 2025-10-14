using FlashcardsApp.Interfaces.Achievements;
using FlashcardsAppContracts.DTOs.Achievements.Responses;

namespace FlashcardsApp.Services.Achievements;

/// <summary>
/// Сервис для начисления бонусов
/// TODO: Реализовать в следующей итерации
/// </summary>
public class AchievementRewardService : IAchievementRewardService
{
    public Task<ServiceResult<AchievementRewardDto>> AwardBonusForAchievementAsync(Guid userId, Guid achievementId)
    {
        throw new NotImplementedException("AchievementRewardService will be implemented in the next iteration");
    }

    public Task<ServiceResult<IEnumerable<AchievementRewardDto>>> GetUserRewardHistoryAsync(Guid userId)
    {
        throw new NotImplementedException("AchievementRewardService will be implemented in the next iteration");
    }
}