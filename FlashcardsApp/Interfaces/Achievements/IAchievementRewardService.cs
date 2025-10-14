using FlashcardsApp.Services;
using FlashcardsAppContracts.DTOs.Achievements.Responses;

namespace FlashcardsApp.Interfaces.Achievements;

/// <summary>
/// Сервис для начисления бонусов за достижения (будущая функциональность)
/// </summary>
public interface IAchievementRewardService
{
    /// <summary>
    /// Начислить бонус за разблокировку достижения
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="achievementId">ID достижения</param>
    /// <returns>Количество начисленного бонуса (XP, валюта и т.д.)</returns>
    Task<ServiceResult<AchievementRewardDto>> AwardBonusForAchievementAsync(Guid userId, Guid achievementId);
}