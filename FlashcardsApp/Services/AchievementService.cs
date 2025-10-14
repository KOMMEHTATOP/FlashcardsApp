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
    public async Task<ServiceResult<IEnumerable<AchievementDto>>> GetAllAchievementsAsync()
    {
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

        return ServiceResult<IEnumerable<AchievementDto>>.Success(achievements);
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

        return ServiceResult<IEnumerable<AchievementWithStatusDto>>.Success(result);
    }
    
    // Разблокировать достижение
    public async Task<ServiceResult<UserAchievementDto>> UnlockAchievementAsync(Guid userId, Guid achievementId)
    {
        var exists = await _context.UserAchievements
            .AnyAsync(ua => ua.UserId == userId && ua.AchievementId == achievementId);

        if (exists)
        {
            return ServiceResult<UserAchievementDto>.Failure("Achievement already unlocked");
        }

        var userAchievement = new UserAchievement
        {
            UserId = userId,
            AchievementId = achievementId,
            UnlockedAt = DateTime.UtcNow
        };

        _context.UserAchievements.Add(userAchievement);
        await _context.SaveChangesAsync();

    
        await _context.Entry(userAchievement)
            .Reference(ua => ua.Achievement)
            .LoadAsync();

        var dto = new UserAchievementDto
        {
            UserId = userAchievement.UserId,
            AchievementId = userAchievement.AchievementId,
            UnlockedAt = userAchievement.UnlockedAt,
            Achievement = new AchievementDto
            {
                Id = userAchievement.Achievement!.Id,
                Name = userAchievement.Achievement.Name,
                Description = userAchievement.Achievement.Description,
                IconUrl = userAchievement.Achievement.IconUrl,
                Gradient = userAchievement.Achievement.Gradient
            }
        };

        return ServiceResult<UserAchievementDto>.Success(dto);
    }

    // Проверить и разблокировать достижения на основе статистики
    public async Task<ServiceResult<List<AchievementDto>>> CheckAndUnlockAchievementsAsync(Guid userId)
    {
        var statistics = await _context.UserStatistics
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (statistics == null)
        {
            return ServiceResult<List<AchievementDto>>.Failure("Statistics not found");
        }

        var allAchievements = await _context.Achievements.ToListAsync();
        var userAchievementIds = await _context.UserAchievements
            .Where(ua => ua.UserId == userId)
            .Select(ua => ua.AchievementId)
            .ToListAsync();

        var newlyUnlocked = new List<AchievementDto>();

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
                    newlyUnlocked.Add(new AchievementDto 
                    {
                        Id = achievement.Id,
                        Name = achievement.Name,
                        Description = achievement.Description,
                        IconUrl = achievement.IconUrl,
                        Gradient = achievement.Gradient
                    });
                }
            }
        }

        return ServiceResult<List<AchievementDto>>.Success(newlyUnlocked);
    }
}