// Services/UserStatisticsService.cs
using FlashcardsApp.Data;
using FlashcardsApp.Models;
using FlashcardsAppContracts.DTOs.Responses;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.Services;

public class UserStatisticsService
{
    private readonly ApplicationDbContext _context;
    private readonly GamificationService _gamificationService;

    public UserStatisticsService(ApplicationDbContext context, GamificationService gamificationService)
    {
        _context = context;
        _gamificationService = gamificationService;
    }
    
    /// <summary>
    /// Получить статистику пользователя с расчетами для фронтенда
    /// </summary>
    public async Task<ServiceResult<UserStatsDto>> GetUserStatsAsync(Guid userId)
    {
        var stats = await _context.UserStatistics
            .AsNoTracking()
            .FirstOrDefaultAsync(us => us.UserId == userId);

        if (stats == null)
        {
            return ServiceResult<UserStatsDto>.Failure("User statistics not found");
        }

        // Рассчитываем XP для уровней
        var xpForCurrentLevel = _gamificationService.CalculateXPForLevel(stats.Level);
        var xpForNextLevel = _gamificationService.CalculateXPForLevel(stats.Level + 1);
        
        var xpNeeded = xpForNextLevel - stats.TotalXP;
        var xpProgressInCurrentLevel = stats.TotalXP - xpForCurrentLevel;
        var xpRequiredForCurrentLevel = xpForNextLevel - xpForCurrentLevel;

        var dto = new UserStatsDto
        {
            TotalXP = stats.TotalXP,
            Level = stats.Level,
            XPForNextLevel = xpNeeded,
            XPProgressInCurrentLevel = xpProgressInCurrentLevel,
            XPRequiredForCurrentLevel = xpRequiredForCurrentLevel,
            CurrentStreak = stats.CurrentStreak,
            BestStreak = stats.BestStreak,
            TotalStudyTime = stats.TotalStudyTime
        };

        return ServiceResult<UserStatsDto>.Success(dto);
    }

    /// <summary>
    /// Инициализировать статистику для нового пользователя
    /// </summary>
    public async Task<ServiceResult<bool>> CreateInitialStatisticsAsync(Guid userId)
    {
        var existingStats = await _context.UserStatistics
            .AnyAsync(us => us.UserId == userId);

        if (existingStats)
        {
            return ServiceResult<bool>.Failure("User statistics already exist");
        }

        var statistics = new UserStatistics
        {
            UserId = userId,
            TotalXP = 0,
            Level = 1,
            CurrentStreak = 0,
            BestStreak = 0,
            LastStudyDate = DateTime.UtcNow,
            TotalStudyTime = TimeSpan.Zero
        };

        _context.UserStatistics.Add(statistics);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Success(true);
    }
}