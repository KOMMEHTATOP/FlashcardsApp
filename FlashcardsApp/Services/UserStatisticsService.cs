using FlashcardsApp.Data;
using FlashcardsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.Services;

public class UserStatisticsService
{
    private readonly ApplicationDbContext _context;

    public UserStatisticsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<UserStatistics>> GetUserStatisticsAsync(Guid userId)
    {
        var statistics = await _context.UserStatistics
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (statistics == null)
        {
            return ServiceResult<UserStatistics>.Failure("Statistics not found");
        }

        return ServiceResult<UserStatistics>.Success(statistics);
    }

    public async Task<ServiceResult<UserStatistics>> CreateInitialStatisticsAsync(Guid userId)
    {
        var statistics = new UserStatistics
        {
            UserId = userId,
            TotalXP = 0,
            CurrentStreak = 0,
            BestStreak = 0,
            LastStudyDate = DateTime.UtcNow,
            TotalStudyTime = TimeSpan.Zero
        };

        _context.UserStatistics.Add(statistics);
        await _context.SaveChangesAsync();

        return ServiceResult<UserStatistics>.Success(statistics);
    }

    public async Task<ServiceResult<UserStatistics>> UpdateStatisticsAsync(Guid userId, int xpGained, TimeSpan studyTime)
    {
        var statistics = await _context.UserStatistics
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (statistics == null)
        {
            return ServiceResult<UserStatistics>.Failure("Statistics not found");
        }

        statistics.TotalXP += xpGained;
        statistics.TotalStudyTime += studyTime;

        // Проверка streak
        var today = DateTime.UtcNow.Date;
        var lastStudy = statistics.LastStudyDate.Date;

        if (today == lastStudy)
        {
            // Уже занимался сегодня, streak не меняется
        }
        else if (today == lastStudy.AddDays(1))
        {
            // Продолжение streak
            statistics.CurrentStreak++;
            if (statistics.CurrentStreak > statistics.BestStreak)
            {
                statistics.BestStreak = statistics.CurrentStreak;
            }
        }
        else
        {
            // Streak прервался
            statistics.CurrentStreak = 1;
        }

        statistics.LastStudyDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return ServiceResult<UserStatistics>.Success(statistics);
    }
}