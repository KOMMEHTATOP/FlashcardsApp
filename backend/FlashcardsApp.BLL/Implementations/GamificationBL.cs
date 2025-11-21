using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.DAL;
using FlashcardsApp.Models.Constants;
using FlashcardsApp.Models.DTOs.Statistics.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FlashcardsApp.BLL.Implementations;

public class GamificationBL : IGamificationBL
{
    private readonly ApplicationDbContext _context;
    private readonly RewardSettings _settings;

    public GamificationBL(
        ApplicationDbContext context,
        IOptions<RewardSettings> settingsOptions)
    {
        _context = context;
        _settings = settingsOptions.Value;
    }
    
    public async Task<int> CalculateXPForCardAsync(Guid userId, Guid cardId, int rating)
    {
        var baseXP = _settings.Base.XPPerCard;

        var qualityMultiplier = CalculateQualityMultiplier(rating);

        var userStats = await _context.UserStatistics
            .AsNoTracking()
            .FirstOrDefaultAsync(us => us.UserId == userId);
        
        var streakBonus = CalculateStreakBonus(userStats?.CurrentStreak ?? 0);

        var xp = (int)Math.Round(baseXP * qualityMultiplier * streakBonus);

        return xp;
    }
    
    private double CalculateQualityMultiplier(int rating)
    {
        return rating switch
        {
            5 => _settings.Multipliers.Quality.Rating5,
            4 => _settings.Multipliers.Quality.Rating4,
            3 => _settings.Multipliers.Quality.Rating3,
            2 => _settings.Multipliers.Quality.Rating2,
            1 => _settings.Multipliers.Quality.Rating1,
            _ => _settings.Multipliers.Quality.Rating3 
        };
    }
    
    private double CalculateStreakBonus(int currentStreak)
    {
        return currentStreak switch
        {
            >= 30 => _settings.Multipliers.StreakBonus.Days30Plus,  
            >= 14 => _settings.Multipliers.StreakBonus.Days14Plus,  
            >= 7 => _settings.Multipliers.StreakBonus.Days7Plus,    
            _ => _settings.Multipliers.StreakBonus.Default      
        };
    }
    
    public async Task<ServiceResult<MotivationalMessageDto>> GetMotivationalMessageAsync(Guid userId)
    {
        var userStats = await _context.UserStatistics
            .AsNoTracking()
            .FirstOrDefaultAsync(us => us.UserId == userId);

        if (userStats == null)
        {
            return ServiceResult<MotivationalMessageDto>.Failure("User statistics not found");
        }

        var xpForNextLevel = CalculateXPForLevel(userStats.Level + 1);
        var xpNeeded = xpForNextLevel - userStats.TotalXP;

        MotivationalMessageDto message;

        if (xpNeeded < 200)
        {
            var cardsNeeded = (int)Math.Ceiling(xpNeeded / (double)_settings.Base.XPPerCard);
            message = new MotivationalMessageDto
            {
                Message = $"Почти там! Изучи еще {cardsNeeded} карточек для уровня {userStats.Level + 1}!",
                Icon = "🚀",
                Type = "level"
            };
        }
        else if (userStats.CurrentStreak >= 7)
        {
            message = new MotivationalMessageDto
            {
                Message = $"Отличный streak! {userStats.CurrentStreak} дней подряд! Так держать!",
                Icon = "🔥",
                Type = "streak"
            };
        }
        else
        {
            message = new MotivationalMessageDto
            {
                Message = $"Продолжай идти! Всего {xpNeeded} очков до уровня {userStats.Level + 1}!",
                Icon = "🎯",
                Type = "level"
            };
        }

        return ServiceResult<MotivationalMessageDto>.Success(message);
    }
    
    public async Task<ServiceResult<(bool leveledUp, int newLevel)>> AddXPToUserAsync(Guid userId, int xp)
    {
        var userStats = await _context.UserStatistics.FirstOrDefaultAsync(us => us.UserId == userId);
        
        if (userStats == null)
        {
            return ServiceResult<(bool, int)>.Failure("User statistics not found");
        }

        var oldLevel = userStats.Level;
        userStats.TotalXP += xp;

        var newLevel = CalculateLevelFromXP(userStats.TotalXP);
        var leveledUp = newLevel > oldLevel;

        if (leveledUp)
        {
            userStats.Level = newLevel;
        }

        await _context.SaveChangesAsync();

        return ServiceResult<(bool leveledUp, int newLevel)>.Success((leveledUp, newLevel));
    }
    
    public int CalculateXPForLevel(int level)
    {
        if (level == 1) return 0;
        if (level <= 10) return 100 * level;  
        if (level <= 25) return 100 * level + 50 * (level - 10);
        return 100 * level + 50 * 15 + 100 * (level - 25);
    }
    
    private int CalculateLevelFromXP(int totalXP)
    {
        int level = 1;
        while (CalculateXPForLevel(level + 1) <= totalXP)
        {
            level++;
        }
        return level;
    }
    
    public async Task<ServiceResult<bool>> UpdateStreakAsync(Guid userId)
    {
        var userStats = await _context.UserStatistics.FirstOrDefaultAsync(us => us.UserId == userId);
        
        if (userStats == null)
        {
            return ServiceResult<bool>.Failure("User statistics not found");
        }

        var today = DateTime.UtcNow.Date;
        var lastStudyDate = userStats.LastStudyDate.Date;
        var daysDifference = (today - lastStudyDate).Days;

        bool streakIncreased = false;

        if (daysDifference == 0)
        {
            streakIncreased = false;
        }
        else if (daysDifference == 1)
        {
            userStats.CurrentStreak++;
            streakIncreased = true;
            
            if (userStats.CurrentStreak > userStats.BestStreak)
                userStats.BestStreak = userStats.CurrentStreak;
        }
        else
        {
            userStats.CurrentStreak = 1;
            streakIncreased = false;
        }

        userStats.LastStudyDate = today;
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Success(streakIncreased);
    }
}