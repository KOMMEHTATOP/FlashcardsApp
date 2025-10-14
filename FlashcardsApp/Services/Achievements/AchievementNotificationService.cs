using FlashcardsApp.Interfaces.Achievements;

namespace FlashcardsApp.Services.Achievements;

/// <summary>
/// Сервис для отправки уведомлений
/// TODO: Реализовать в следующей итерации
/// </summary>
public class AchievementNotificationService : IAchievementNotificationService
{
    public Task<ServiceResult<bool>> SendAchievementUnlockedNotificationAsync(Guid userId, Guid achievementId)
    {
        throw new NotImplementedException("AchievementNotificationService will be implemented in the next iteration");
    }

    public Task<ServiceResult<bool>> SendAchievementReminderAsync(Guid userId, Guid achievementId)
    {
        throw new NotImplementedException("AchievementNotificationService will be implemented in the next iteration");
    }

    public Task<ServiceResult<bool>> SendWeeklyAchievementSummaryAsync(Guid userId)
    {
        throw new NotImplementedException("AchievementNotificationService will be implemented in the next iteration");
    }
}
