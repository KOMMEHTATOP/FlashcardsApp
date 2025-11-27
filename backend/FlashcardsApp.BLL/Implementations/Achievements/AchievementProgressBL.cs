using FlashcardsApp.BLL.Interfaces.Achievements;
using FlashcardsApp.DAL;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.DTOs.Achievements.Responses;
using FlashcardsApp.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FlashcardsApp.BLL.Implementations.Achievements;

public class AchievementProgressBL : IAchievementProgressBL
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AchievementProgressBL> _logger;

    public AchievementProgressBL(
        ApplicationDbContext context,
        ILogger<AchievementProgressBL> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResult<AchievementProgressDto>> CalculateAchievementProgressAsync(
        Guid userId, 
        Guid achievementId)
    {
        try
        {
            var achievement = await _context.Achievements
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == achievementId && a.IsActive);

            if (achievement == null)
            {
                return ServiceResult<AchievementProgressDto>.Failure("Достижение не найдено");
            }

            var userStats = await _context.UserStatistics
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (userStats == null)
            {
                return ServiceResult<AchievementProgressDto>.Failure("Статистика пользователя не найдена");
            }

            var userAchievement = await _context.UserAchievements
                .AsNoTracking()
                .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AchievementId == achievementId);

            var currentValue = GetCurrentValueForCondition(achievement.ConditionType, userStats);
            var progressPercentage = CalculateProgressPercentage(currentValue, achievement.ConditionValue);

            var progressDto = new AchievementProgressDto
            {
                AchievementId = achievement.Id,
                Name = achievement.Name,
                Description = achievement.Description,
                IconUrl = achievement.IconUrl,
                Gradient = achievement.Gradient,
                ConditionType = achievement.ConditionType,
                Rarity = achievement.Rarity,
                ConditionValue = achievement.ConditionValue,
                CurrentValue = currentValue,
                ProgressPercentage = progressPercentage,
                IsUnlocked = userAchievement != null,
                UnlockedAt = userAchievement?.UnlockedAt
            };

            return ServiceResult<AchievementProgressDto>.Success(progressDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Ошибка при расчете прогресса достижения {AchievementId} для пользователя {UserId}", 
                achievementId, userId);
            return ServiceResult<AchievementProgressDto>.Failure("Ошибка при расчете прогресса достижения");
        }
    }

    public async Task<ServiceResult<IEnumerable<AchievementProgressDto>>> GetAllAchievementsProgressAsync(
        Guid userId)
    {
        try
        {
            var userStats = await _context.UserStatistics
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (userStats == null)
            {
                return ServiceResult<IEnumerable<AchievementProgressDto>>.Failure(
                    "Статистика пользователя не найдена");
            }

            var achievements = await _context.Achievements
                .AsNoTracking()
                .Where(a => a.IsActive)
                .OrderBy(a => a.DisplayOrder)
                .ToListAsync();

            var unlockedAchievements = await _context.UserAchievements
                .AsNoTracking()
                .Where(ua => ua.UserId == userId)
                .ToDictionaryAsync(ua => ua.AchievementId, ua => ua.UnlockedAt);

            var progressList = achievements.Select(achievement =>
            {
                var currentValue = GetCurrentValueForCondition(achievement.ConditionType, userStats);
                var progressPercentage = CalculateProgressPercentage(currentValue, achievement.ConditionValue);
                var isUnlocked = unlockedAchievements.ContainsKey(achievement.Id);

                return new AchievementProgressDto
                {
                    AchievementId = achievement.Id,
                    Name = achievement.Name,
                    Description = achievement.Description,
                    IconUrl = achievement.IconUrl,
                    Gradient = achievement.Gradient,
                    ConditionType = achievement.ConditionType,
                    Rarity = achievement.Rarity,
                    ConditionValue = achievement.ConditionValue,
                    CurrentValue = currentValue,
                    ProgressPercentage = progressPercentage,
                    IsUnlocked = isUnlocked,
                    UnlockedAt = isUnlocked ? unlockedAchievements[achievement.Id] : null
                };
            }).ToList();

            return ServiceResult<IEnumerable<AchievementProgressDto>>.Success(progressList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Ошибка при получении прогресса всех достижений для пользователя {UserId}", userId);
            return ServiceResult<IEnumerable<AchievementProgressDto>>.Failure(
                "Ошибка при получении прогресса достижений");
        }
    }

    private static int GetCurrentValueForCondition(
        AchievementConditionType conditionType, 
        UserStatistics stats)
    {
        return conditionType switch
        {
            AchievementConditionType.TotalCardsStudied => stats.TotalCardsStudied,
            AchievementConditionType.TotalCardsCreated => stats.TotalCardsCreated,
            AchievementConditionType.CurrentStreak => stats.CurrentStreak,
            AchievementConditionType.BestStreak => stats.BestStreak,
            AchievementConditionType.Level => stats.Level,
            AchievementConditionType.TotalXP => stats.TotalXP,
            AchievementConditionType.PerfectRatingsStreak => stats.PerfectRatingsStreak,
            AchievementConditionType.TotalStudyTimeHours => (int)stats.TotalStudyTime.TotalHours,
            _ => 0
        };
    }

    private static int CalculateProgressPercentage(int currentValue, int targetValue)
    {
        if (targetValue <= 0)
            return 0;

        var percentage = (int)Math.Round((double)currentValue / targetValue * 100);
        return Math.Min(100, Math.Max(0, percentage));
    }
}