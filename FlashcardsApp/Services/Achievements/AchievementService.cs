using FlashcardsApp.Data;
using FlashcardsApp.Interfaces.Achievements;
using FlashcardsApp.Models;
using FlashcardsAppContracts.Constants;
using FlashcardsAppContracts.DTOs.Achievements.Responses;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.Services.Achievements;

public class AchievementService : IAchievementService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AchievementService> _logger;

    public AchievementService(
        ApplicationDbContext context,
        ILogger<AchievementService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<AchievementDto>>> GetAllAchievementsAsync()
    {
        _logger.LogInformation("Fetching all achievements");

        var achievements = await _context.Achievements
            .AsNoTracking()
            .Select(a => new AchievementDto
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                IconUrl = a.IconUrl,
                Gradient = a.Gradient
            })
            .ToListAsync();

        _logger.LogDebug("Retrieved {Count} achievements", achievements.Count);
        return ServiceResult<IEnumerable<AchievementDto>>.Success(achievements);
    }

    public async Task<ServiceResult<IEnumerable<UnlockedAchievementDto>>> GetUserAchievementsAsync(Guid userId)
    {
        _logger.LogInformation("Fetching unlocked achievements for user {UserId}", userId);

        var userAchievements = await _context.UserAchievements
            .AsNoTracking()
            .Include(ua => ua.Achievement)
            .Where(ua => ua.UserId == userId)
            .Select(ua => new UnlockedAchievementDto
            {
                Id = ua.Achievement.Id,
                Name = ua.Achievement.Name,
                Description = ua.Achievement.Description,
                IconUrl = ua.Achievement.IconUrl,
                Gradient = ua.Achievement.Gradient
            })
            .ToListAsync();

        _logger.LogDebug("User {UserId} has {Count} unlocked achievements", userId, userAchievements.Count);
        return ServiceResult<IEnumerable<UnlockedAchievementDto>>.Success(userAchievements);
    }

    public async Task<ServiceResult<IEnumerable<AchievementWithStatusDto>>> GetAllAchievementsWithStatusAsync(Guid userId)
    {
        _logger.LogInformation("Fetching all achievements with status for user {UserId}", userId);

        var result = await _context.Achievements
            .AsNoTracking()
            .Select(a => new AchievementWithStatusDto
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                IconUrl = a.IconUrl,
                Gradient = a.Gradient,
                IsUnlocked = a.UserAchievements.Any(ua => ua.UserId == userId)
            })
            .ToListAsync();

        _logger.LogDebug("Retrieved {Total} achievements, {Unlocked} unlocked for user {UserId}",
            result.Count,
            result.Count(a => a.IsUnlocked),
            userId);

        return ServiceResult<IEnumerable<AchievementWithStatusDto>>.Success(result);
    }

    public async Task<ServiceResult<UserAchievementDto>> UnlockAchievementAsync(Guid userId, Guid achievementId)
    {
        _logger.LogInformation("Attempting to unlock achievement {AchievementId} for user {UserId}", achievementId, userId);

        var exists = await _context.UserAchievements
            .AnyAsync(ua => ua.UserId == userId && ua.AchievementId == achievementId);

        if (exists)
        {
            _logger.LogWarning("Achievement {AchievementId} already unlocked for user {UserId}", achievementId, userId);
            return ServiceResult<UserAchievementDto>.Failure("Achievement already unlocked");
        }

        var achievement = await _context.Achievements
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == achievementId);

        if (achievement == null)
        {
            _logger.LogWarning("Achievement {AchievementId} not found", achievementId);
            return ServiceResult<UserAchievementDto>.Failure("Achievement not found");
        }

        var userAchievement = new UserAchievement
        {
            UserId = userId,
            AchievementId = achievementId,
            UnlockedAt = DateTime.UtcNow
        };

        _context.UserAchievements.Add(userAchievement);

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Achievement '{AchievementName}' unlocked for user {UserId}", achievement.Name, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlocking achievement {AchievementId} for user {UserId}", achievementId, userId);
            return ServiceResult<UserAchievementDto>.Failure("Failed to unlock achievement");
        }

        var dto = new UserAchievementDto
        {
            UserId = userAchievement.UserId,
            AchievementId = userAchievement.AchievementId,
            UnlockedAt = userAchievement.UnlockedAt,
            Achievement = new AchievementDto
            {
                Id = achievement.Id,
                Name = achievement.Name,
                Description = achievement.Description,
                IconUrl = achievement.IconUrl,
                Gradient = achievement.Gradient
            }
        };

        return ServiceResult<UserAchievementDto>.Success(dto);
    }

    public async Task<ServiceResult<List<AchievementDto>>> CheckAndUnlockAchievementsAsync(Guid userId)
    {
        _logger.LogInformation("Checking achievements for user {UserId}", userId);

        var statistics = await _context.UserStatistics
            .FirstOrDefaultAsync(s => s.UserId == userId);

        // Если статистики нет - создаем с нулевыми значениями
        if (statistics == null)
        {
            _logger.LogInformation("Statistics not found for user {UserId}, creating new statistics", userId);
            
            statistics = new UserStatistics
            {
                UserId = userId,
                TotalXP = 0,
                Level = 1,
                CurrentStreak = 0,
                BestStreak = 0,
                TotalStudyTime = TimeSpan.Zero,
                TotalCardsStudied = 0,
                TotalCardsCreated = 0,
                LastStudyDate = DateTime.UtcNow,
                PerfectRatingsStreak = 0
            };

            _context.UserStatistics.Add(statistics);
            
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Statistics created for user {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating statistics for user {UserId}", userId);
                return ServiceResult<List<AchievementDto>>.Failure("Failed to create user statistics");
            }
        }

        var allAchievements = await _context.Achievements.AsNoTracking().ToListAsync();
        var unlockedAchievementIds = await _context.UserAchievements
            .AsNoTracking()
            .Where(ua => ua.UserId == userId)
            .Select(ua => ua.AchievementId)
            .ToListAsync();

        var newlyUnlocked = new List<AchievementDto>();
        var achievementsToCheck = allAchievements
            .Where(a => !unlockedAchievementIds.Contains(a.Id))
            .ToList();

        _logger.LogDebug("Checking {Count} achievements for user {UserId}", achievementsToCheck.Count, userId);

        foreach (var achievement in achievementsToCheck)
        {
            if (ShouldUnlockAchievement(achievement, statistics))
            {
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
                    Gradient = achievement.Gradient
                });

                _logger.LogInformation("Achievement '{AchievementName}' auto-unlocked for user {UserId}", achievement.Name, userId);
            }
        }

        if (newlyUnlocked.Any())
        {
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("{Count} new achievements unlocked for user {UserId}", newlyUnlocked.Count, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving unlocked achievements for user {UserId}", userId);
                return ServiceResult<List<AchievementDto>>.Failure("Failed to unlock achievements");
            }
        }
        else
        {
            _logger.LogDebug("No new achievements to unlock for user {UserId}", userId);
        }

        return ServiceResult<List<AchievementDto>>.Success(newlyUnlocked);
    }

    private bool ShouldUnlockAchievement(Achievement achievement, UserStatistics statistics)
    {
        return achievement.Name switch
        {
            AchievementNames.FirstSteps => statistics.TotalXP >= AchievementCriteria.FirstStepsXP,
            AchievementNames.SevenDayStreak => statistics.CurrentStreak >= AchievementCriteria.SevenDayStreakDays,
            AchievementNames.WeeklyActivity => statistics.BestStreak >= AchievementCriteria.WeeklyActivityDays,
            AchievementNames.KnowledgeKing => statistics.Level >= AchievementCriteria.KnowledgeKingLevel,
            AchievementNames.RisingStar => statistics.TotalXP >= AchievementCriteria.RisingStarXP,
            _ => false
        };
    }
}
