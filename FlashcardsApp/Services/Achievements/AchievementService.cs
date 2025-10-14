using FlashcardsApp.Data;
using FlashcardsApp.Interfaces.Achievements;
using FlashcardsApp.Models;
using FlashcardsAppContracts.DTOs.Achievements.Responses;
using FlashcardsAppContracts.Enums;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.Services.Achievements;

/// <summary>
/// Сервис для управления достижениями пользователей
/// </summary>
public class AchievementService : IAchievementService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AchievementService> _logger;
    private readonly IAchievementRewardService _rewardService;

    public AchievementService(
        ApplicationDbContext context,
        ILogger<AchievementService> logger,
        IAchievementRewardService rewardService)
    {
        _context = context;
        _logger = logger;
        _rewardService = rewardService;
    }

    /// <summary>
    /// Получить список всех доступных достижений в системе
    /// </summary>
    public async Task<ServiceResult<IEnumerable<AchievementDto>>> GetAllAchievementsAsync()
    {
        try
        {
            var achievements = await _context.Achievements
                .AsNoTracking()
                .Where(a => a.IsActive)
                .OrderBy(a => a.DisplayOrder)
                .Select(a => new AchievementDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    IconUrl = a.IconUrl,
                    Gradient = a.Gradient,
                    ConditionType = a.ConditionType,
                    ConditionValue = a.ConditionValue,
                    Rarity = a.Rarity
                })
                .ToListAsync();

            return ServiceResult<IEnumerable<AchievementDto>>.Success(achievements);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all achievements");
            return ServiceResult<IEnumerable<AchievementDto>>.Failure("Failed to fetch achievements");
        }
    }

    /// <summary>
    /// Получить список разблокированных достижений пользователя
    /// </summary>
    public async Task<ServiceResult<IEnumerable<UnlockedAchievementDto>>> GetUserAchievementsAsync(Guid userId)
    {
        try
        {
            var unlockedAchievements = await _context.UserAchievements
                .AsNoTracking()
                .Where(ua => ua.UserId == userId)
                .Include(ua => ua.Achievement)
                .OrderByDescending(ua => ua.UnlockedAt)
                .Select(ua => new UnlockedAchievementDto
                {
                    Id = ua.Achievement!.Id,
                    Name = ua.Achievement.Name,
                    Description = ua.Achievement.Description,
                    IconUrl = ua.Achievement.IconUrl,
                    Gradient = ua.Achievement.Gradient,
                    Rarity = ua.Achievement.Rarity,
                    UnlockedAt = ua.UnlockedAt
                })
                .ToListAsync();

            return ServiceResult<IEnumerable<UnlockedAchievementDto>>.Success(unlockedAchievements);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user achievements for user {UserId}", userId);
            return ServiceResult<IEnumerable<UnlockedAchievementDto>>.Failure("Failed to fetch user achievements");
        }
    }

    /// <summary>
    /// Получить все достижения со статусом разблокировки для конкретного пользователя
    /// </summary>
    public async Task<ServiceResult<IEnumerable<AchievementWithStatusDto>>> GetAllAchievementsWithStatusAsync(Guid userId)
    {
        try
        {
            var allAchievements = await _context.Achievements
                .AsNoTracking()
                .Where(a => a.IsActive)
                .OrderBy(a => a.DisplayOrder)
                .ToListAsync();

            var unlockedIds = await _context.UserAchievements
                .AsNoTracking()
                .Where(ua => ua.UserId == userId)
                .Select(ua => ua.AchievementId)
                .ToListAsync();

            var achievementsWithStatus = allAchievements.Select(a => new AchievementWithStatusDto
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                IconUrl = a.IconUrl,
                Gradient = a.Gradient,
                ConditionType = a.ConditionType,
                ConditionValue = a.ConditionValue,
                Rarity = a.Rarity,
                IsUnlocked = unlockedIds.Contains(a.Id)
            });

            return ServiceResult<IEnumerable<AchievementWithStatusDto>>.Success(achievementsWithStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching achievements with status for user {UserId}", userId);
            return ServiceResult<IEnumerable<AchievementWithStatusDto>>.Failure("Failed to fetch achievements with status");
        }
    }

    /// <summary>
    /// Разблокировать конкретное достижение для пользователя вручную
    /// </summary>
    public async Task<ServiceResult<UserAchievementDto>> UnlockAchievementAsync(Guid userId, Guid achievementId)
    {
        try
        {
            // Проверяем что достижение существует
            var achievement = await _context.Achievements
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == achievementId && a.IsActive);

            if (achievement == null)
            {
                return ServiceResult<UserAchievementDto>.Failure("Achievement not found or inactive");
            }

            // Проверяем что уже не разблокировано
            var alreadyUnlocked = await _context.UserAchievements
                .AnyAsync(ua => ua.UserId == userId && ua.AchievementId == achievementId);

            if (alreadyUnlocked)
            {
                return ServiceResult<UserAchievementDto>.Failure("Achievement already unlocked");
            }

            // Разблокируем достижение
            var userAchievement = new UserAchievement
            {
                UserId = userId,
                AchievementId = achievementId,
                UnlockedAt = DateTime.UtcNow
            };

            _context.UserAchievements.Add(userAchievement);
            await _context.SaveChangesAsync();

            // Начисляем награду за достижение
            await _rewardService.AwardBonusForAchievementAsync(userId, achievementId);

            _logger.LogInformation(
                "Achievement {AchievementName} unlocked for user {UserId}",
                achievement.Name, userId);

            var result = new UserAchievementDto
            {
                AchievementId = achievementId,
                AchievementName = achievement.Name,
                UnlockedAt = userAchievement.UnlockedAt
            };

            return ServiceResult<UserAchievementDto>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlocking achievement {AchievementId} for user {UserId}", 
                achievementId, userId);
            return ServiceResult<UserAchievementDto>.Failure("Failed to unlock achievement");
        }
    }

    /// <summary>
    /// Автоматически проверить статистику пользователя и разблокировать достижения
    /// </summary>
    public async Task<ServiceResult<List<AchievementDto>>> CheckAndUnlockAchievementsAsync(Guid userId)
    {
        try
        {
            // Получаем статистику пользователя
            var stats = await _context.UserStatistics
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (stats == null)
            {
                _logger.LogWarning("User statistics not found for user {UserId}", userId);
                return ServiceResult<List<AchievementDto>>.Success(new List<AchievementDto>());
            }

            // Получаем все активные достижения
            var allAchievements = await _context.Achievements
                .AsNoTracking()
                .Where(a => a.IsActive)
                .ToListAsync();

            // Получаем уже разблокированные достижения
            var unlockedIds = await _context.UserAchievements
                .AsNoTracking()
                .Where(ua => ua.UserId == userId)
                .Select(ua => ua.AchievementId)
                .ToListAsync();

            // Фильтруем достижения которые еще не разблокированы
            var lockedAchievements = allAchievements
                .Where(a => !unlockedIds.Contains(a.Id))
                .ToList();

            // Проверяем условия для каждого достижения
            var newlyUnlocked = new List<AchievementDto>();

            foreach (var achievement in lockedAchievements)
            {
                bool conditionMet = CheckAchievementCondition(achievement, stats);

                if (conditionMet)
                {
                    // Разблокируем достижение
                    var userAchievement = new UserAchievement
                    {
                        UserId = userId,
                        AchievementId = achievement.Id,
                        UnlockedAt = DateTime.UtcNow
                    };

                    _context.UserAchievements.Add(userAchievement);

                    newlyUnlocked.Add(new AchievementDto
                    {
                        Id = achievement.Id,
                        Name = achievement.Name,
                        Description = achievement.Description,
                        IconUrl = achievement.IconUrl,
                        Gradient = achievement.Gradient,
                        ConditionType = achievement.ConditionType,
                        ConditionValue = achievement.ConditionValue,
                        Rarity = achievement.Rarity
                    });

                    _logger.LogInformation(
                        "Achievement {AchievementName} auto-unlocked for user {UserId}",
                        achievement.Name, userId);
                }
            }

            // Сохраняем все новые достижения
            if (newlyUnlocked.Any())
            {
                await _context.SaveChangesAsync();

                // Начисляем награды за каждое достижение
                foreach (var achievement in newlyUnlocked)
                {
                    await _rewardService.AwardBonusForAchievementAsync(userId, achievement.Id);
                }

                _logger.LogInformation(
                    "{Count} new achievements unlocked for user {UserId}",
                    newlyUnlocked.Count, userId);
            }

            return ServiceResult<List<AchievementDto>>.Success(newlyUnlocked);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking achievements for user {UserId}", userId);
            return ServiceResult<List<AchievementDto>>.Failure("Failed to check achievements");
        }
    }

    /// <summary>
    /// Проверяет выполнено ли условие достижения
    /// </summary>
    private bool CheckAchievementCondition(Achievement achievement, UserStatistics stats)
    {
        return achievement.ConditionType switch
        {
            AchievementConditionType.TotalCardsStudied => 
                stats.TotalCardsStudied >= achievement.ConditionValue,
            
            AchievementConditionType.TotalCardsCreated => 
                stats.TotalCardsCreated >= achievement.ConditionValue,
            
            AchievementConditionType.CurrentStreak => 
                stats.CurrentStreak >= achievement.ConditionValue,
            
            AchievementConditionType.BestStreak => 
                stats.BestStreak >= achievement.ConditionValue,
            
            AchievementConditionType.Level => 
                stats.Level >= achievement.ConditionValue,
            
            AchievementConditionType.TotalXP => 
                stats.TotalXP >= achievement.ConditionValue,
            
            AchievementConditionType.PerfectRatingsStreak => 
                stats.PerfectRatingsStreak >= achievement.ConditionValue,
            
            AchievementConditionType.TotalStudyTimeHours => 
                stats.TotalStudyTime.TotalHours >= achievement.ConditionValue,
            
            _ => false
        };
    }
}