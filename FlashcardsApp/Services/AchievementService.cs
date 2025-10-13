using FlashcardsApp.Data;
using FlashcardsApp.Models;
using FlashcardsAppContracts.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.Services;

public class AchievementService
{
    private readonly ApplicationDbContext _context;

    public AchievementService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Получить все достижения
    public async Task<ServiceResult<IEnumerable<Achievement>>> GetAllAchievementsAsync()
    {
        var achievements = await _context.Achievements
            .AsNoTracking()
            .ToListAsync();

        return ServiceResult<IEnumerable<Achievement>>.Success(achievements);
    }

    // Получить ТОЛЬКО разблокированные достижения пользователя
    public async Task<ServiceResult<IEnumerable<UnlockedAchievementDto>>> GetUserAchievementsAsync(Guid userId)
    {
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

        return ServiceResult<IEnumerable<UnlockedAchievementDto>>.Success(userAchievements);
    }

    // Получить все достижения со статусом разблокировки для юзера
    public async Task<ServiceResult<IEnumerable<AchievementWithStatusDto>>> GetAllAchievementsWithStatusAsync(Guid userId)
    {
        var allAchievements = await _context.Achievements
            .AsNoTracking()
            .ToListAsync();

        var unlockedAchievementIds = (await _context.UserAchievements
            .AsNoTracking()
            .Where(ua => ua.UserId == userId)
            .Select(ua => ua.AchievementId)
            .ToListAsync())
            .ToHashSet();

        var result = allAchievements.Select(a => new AchievementWithStatusDto
        {
            Id = a.Id,
            Name = a.Name,
            Description = a.Description,
            IconUrl = a.IconUrl,
            Gradient = a.Gradient,
            IsUnlocked = unlockedAchievementIds.Contains(a.Id)
        }).ToList();

        return ServiceResult<IEnumerable<AchievementWithStatusDto>>.Success(result);
    }

    // Разблокировать достижение
    public async Task<ServiceResult<UserAchievement>> UnlockAchievementAsync(Guid userId, Guid achievementId)
    {
        var exists = await _context.UserAchievements
            .AnyAsync(ua => ua.UserId == userId && ua.AchievementId == achievementId);

        if (exists)
        {
            return ServiceResult<UserAchievement>.Failure("Achievement already unlocked");
        }

        var userAchievement = new UserAchievement
        {
            UserId = userId,
            AchievementId = achievementId,
            UnlockedAt = DateTime.UtcNow
        };

        _context.UserAchievements.Add(userAchievement);
        await _context.SaveChangesAsync();

        return ServiceResult<UserAchievement>.Success(userAchievement);
    }

    // Проверить и разблокировать достижения на основе статистики
    public async Task<ServiceResult<List<Achievement>>> CheckAndUnlockAchievementsAsync(Guid userId)
    {
        var statistics = await _context.UserStatistics
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (statistics == null)
        {
            return ServiceResult<List<Achievement>>.Failure("Statistics not found");
        }

        var allAchievements = await _context.Achievements.ToListAsync();
        var userAchievementIds = await _context.UserAchievements
            .Where(ua => ua.UserId == userId)
            .Select(ua => ua.AchievementId)
            .ToListAsync();

        var newlyUnlocked = new List<Achievement>();

        foreach (var achievement in allAchievements)
        {
            if (userAchievementIds.Contains(achievement.Id))
                continue;

            bool shouldUnlock = achievement.Name switch
            {
                "Первые шаги" => statistics.TotalXP >= 10,
                "7 дней подряд" => statistics.CurrentStreak >= 7,
                "Неделя активности" => statistics.BestStreak >= 7,
                "Король знаний" => statistics.Level >= 10,
                "Восходящая звезда" => statistics.TotalXP >= 1000,
                _ => false
            };

            if (shouldUnlock)
            {
                var unlockResult = await UnlockAchievementAsync(userId, achievement.Id);
                if (unlockResult.IsSuccess)
                {
                    newlyUnlocked.Add(achievement);
                }
            }
        }

        return ServiceResult<List<Achievement>>.Success(newlyUnlocked);
    }
}