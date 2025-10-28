using FlashcardsApp.BLL.Interfaces.Achievements;
using FlashcardsApp.DAL;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.DTOs.Achievements.Responses;
using FlashcardsApp.Models.Enums;
using FlashcardsApp.Models.Notifications;
using FlashcardsApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace FlashcardsApp.BLL.Implementations.Achievements;

public class AchievementService : IAchievementService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AchievementService> _logger;
    private readonly IAchievementRewardService _rewardService;
    private readonly INotificationService _notificationService;

    public AchievementService(
        ApplicationDbContext context,
        ILogger<AchievementService> logger,
        IAchievementRewardService rewardService,
        INotificationService notificationService)
    {
        _context = context;
        _logger = logger;
        _rewardService = rewardService;
        _notificationService = notificationService;
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
            var unlockedAt = DateTime.UtcNow;
            var userAchievement = new UserAchievement
            {
                UserId = userId,
                AchievementId = achievementId,
                UnlockedAt = unlockedAt
            };

            _context.UserAchievements.Add(userAchievement);
            await _context.SaveChangesAsync();

            // Начисляем награду за достижение
            var rewardResult = await _rewardService.AwardBonusForAchievementAsync(userId, achievementId);
            var bonusXP = rewardResult.IsSuccess ? rewardResult.Data.XPAwarded : 0;

            // ОТПРАВЛЯЕМ УВЕДОМЛЕНИЕ через SignalR
            await SendAchievementNotificationAsync(userId, achievement, unlockedAt, bonusXP);

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
            var unlockedAt = DateTime.UtcNow; // ← Единое время для всех

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
                        UnlockedAt = unlockedAt
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

                // ОТПРАВЛЯЕМ УВЕДОМЛЕНИЯ через SignalR
                // Начисляем награды и собираем информацию о бонусах
                var notifications = new List<AchievementUnlockedNotification>();

                foreach (var achievementDto in newlyUnlocked)
                {
                    var rewardResult = await _rewardService.AwardBonusForAchievementAsync(
                        userId, achievementDto.Id);
                    
                    var bonusXP = rewardResult.IsSuccess ? rewardResult.Data.XPAwarded : 0;

                    notifications.Add(new AchievementUnlockedNotification
                    {
                        AchievementId = achievementDto.Id,
                        Name = achievementDto.Name,
                        Description = achievementDto.Description,
                        IconUrl = achievementDto.IconUrl,
                        Gradient = achievementDto.Gradient,
                        Rarity = achievementDto.Rarity.ToString(),
                        UnlockedAt = unlockedAt,
                        BonusXP = bonusXP
                    });
                }

                // Отправляем массовое уведомление
                await _notificationService.SendMultipleAchievementsUnlockedAsync(userId, notifications);

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
    
    /// <summary>
    /// МЕТОД для отправки уведомления
    /// Инкапсулирует создание объекта уведомления
    /// </summary>
    private async Task SendAchievementNotificationAsync(Guid userId, Achievement achievement, DateTime unlockedAt, int bonusXP)
    {
        try
        {
            var notification = new AchievementUnlockedNotification
            {
                AchievementId = achievement.Id,
                Name = achievement.Name,
                Description = achievement.Description,
                IconUrl = achievement.IconUrl,
                Gradient = achievement.Gradient,
                Rarity = achievement.Rarity.ToString(),
                UnlockedAt = unlockedAt,
                BonusXP = bonusXP
            };

            await _notificationService.SendAchievementUnlockedAsync(userId, notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Failed to send achievement notification for achievement {AchievementId} to user {UserId}",
                achievement.Id, userId);
        }
    }

}