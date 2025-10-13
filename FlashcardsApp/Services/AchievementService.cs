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

    // Получить достижения пользователя
    public async Task<ServiceResult<IEnumerable<UserAchievementDto>>> GetUserAchievementsAsync(Guid userId)
    {
        var userAchievements = await _context.UserAchievements
            .AsNoTracking()
            .Include(ua => ua.Achievement)
            .Where(ua => ua.UserId == userId)
            .Select(ua => new UserAchievementDto
            {
                UserId = ua.UserId,
                AchievementId = ua.AchievementId,
                UnlockedAt = ua.UnlockedAt,
                Achievement = new AchievementDto
                {
                    Id = ua.Achievement.Id,
                    Name = ua.Achievement.Name,
                    Description = ua.Achievement.Description,
                    IconUrl = ua.Achievement.IconUrl
                }
            })
            .ToListAsync();

        return ServiceResult<IEnumerable<UserAchievementDto>>.Success(userAchievements);
    }

    // Разблокировать достижение
    public async Task<ServiceResult<UserAchievement>> UnlockAchievementAsync(Guid userId, Guid achievementId)
    {
        // Проверка существует ли уже это достижение у пользователя
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
        var userAchievements = await _context.UserAchievements
            .Where(ua => ua.UserId == userId)
            .Select(ua => ua.AchievementId)
            .ToListAsync();

        var newlyUnlocked = new List<Achievement>();

        foreach (var achievement in allAchievements)
        {
            // Пропустить уже разблокированные
            if (userAchievements.Contains(achievement.Id))
                continue;

            // Здесь логика проверки условий достижений
            bool shouldUnlock = achievement.Name switch
            {
                "Первые шаги" => statistics.TotalXP >= 10,
                "7 дней подряд" => statistics.CurrentStreak >= 7,
                "Высший балл" => false, // Это проверяется отдельно через CardRatings
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