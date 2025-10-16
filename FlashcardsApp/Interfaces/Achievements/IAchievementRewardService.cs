using FlashcardsApp.Services;
using FlashcardsAppContracts.DTOs.Achievements.Responses;

namespace FlashcardsApp.Interfaces.Achievements;

/// <summary>
/// Сервис для начисления наград за достижения
/// </summary>
public interface IAchievementRewardService
{
    /// <summary>
    /// Начислить награду за выполнение достижения (XP).
    /// </summary>
    Task<ServiceResult<AchievementRewardDto>> AwardBonusForAchievementAsync(Guid userId, Guid achievementId);
}
