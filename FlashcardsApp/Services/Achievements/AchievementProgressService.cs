using FlashcardsApp.Interfaces.Achievements;
using FlashcardsAppContracts.DTOs.Achievements.Responses;

namespace FlashcardsApp.Services.Achievements;

/// <summary>
/// Сервис для вычисления прогресса достижений
/// TODO: Реализовать в следующей итерации
/// </summary>
public class AchievementProgressService : IAchievementProgressService
{
    public Task<ServiceResult<AchievementProgressDto>> CalculateAchievementProgressAsync(Guid userId, Guid achievementId)
    {
        throw new NotImplementedException("AchievementProgressService will be implemented in the next iteration");
    }

    public Task<ServiceResult<IEnumerable<AchievementProgressDto>>> GetAllAchievementsProgressAsync(Guid userId)
    {
        throw new NotImplementedException("AchievementProgressService will be implemented in the next iteration");
    }

    public Task<ServiceResult<IEnumerable<AchievementRecommendationDto>>> GetAchievementRecommendationsAsync(Guid userId, int count = 3)
    {
        throw new NotImplementedException("AchievementProgressService will be implemented in the next iteration");
    }
}
