using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.DAL;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.DTOs.Statistics.Responses;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.BLL.Implementations;

public class UserStatisticsBL: IUserStatisticsBL
{
    private readonly ApplicationDbContext _context;
    private readonly IGamificationBL _gamificationBl;

    public UserStatisticsBL(ApplicationDbContext context, IGamificationBL gamificationBl)
    {
        _context = context;
        _gamificationBl = gamificationBl;
    }

    public async Task<ServiceResult<UserStatsDto>> GetUserStatsAsync(Guid userId)
    {
        var stats = await _context.UserStatistics
            .FirstOrDefaultAsync(us => us.UserId == userId);

        if (stats == null)
        {
            stats = new UserStatistics
            {
                UserId = userId,
                TotalXP = 0,
                Level = 1,
                CurrentStreak = 0,
                BestStreak = 0,
                LastStudyDate = DateTime.UtcNow,
                TotalStudyTime = TimeSpan.Zero,
                TotalCardsStudied = 0, 
                TotalCardsCreated = 0, 
                PerfectRatingsStreak = 0 
            };

            _context.UserStatistics.Add(stats);
            await _context.SaveChangesAsync();
        }

        var xpForCurrentLevel = _gamificationBl.CalculateXPForLevel(stats.Level);
        var xpForNextLevel = _gamificationBl.CalculateXPForLevel(stats.Level + 1);

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
            TotalStudyTime = stats.TotalStudyTime,
            TotalCardsStudied = stats.TotalCardsStudied,
            TotalCardsCreated = stats.TotalCardsCreated,
            PerfectRatingsStreak = stats.PerfectRatingsStreak
        };

        return ServiceResult<UserStatsDto>.Success(dto);
    }
}
