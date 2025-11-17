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

    /// <summary>
    /// Рассчитывает XP за изученную карточку
    /// Формула: XP = BaseXP × Quality × Streak
    /// </summary>
    public async Task<int> CalculateXPForCardAsync(Guid userId, Guid cardId, int rating)
    {
        // 1. Базовое XP из конфига
        var baseXP = _settings.Base.XPPerCard;

        // 2. Рассчитываем множитель качества (оценка пользователя)
        var qualityMultiplier = CalculateQualityMultiplier(rating);

        // 3. Получаем streak пользователя для бонуса
        var userStats = await _context.UserStatistics
            .AsNoTracking()
            .FirstOrDefaultAsync(us => us.UserId == userId);
        
        var streakBonus = CalculateStreakBonus(userStats?.CurrentStreak ?? 0);

        // 4. Итоговый расчет: XP = BaseXP × Quality × Streak
        var xp = (int)Math.Round(baseXP * qualityMultiplier * streakBonus);

        return xp;
    }

    /// <summary>
    /// Множитель на основе оценки (1-5)
    /// </summary>
    private double CalculateQualityMultiplier(int rating)
    {
        return rating switch
        {
            5 => _settings.Multipliers.Quality.Rating5,
            4 => _settings.Multipliers.Quality.Rating4,
            3 => _settings.Multipliers.Quality.Rating3,
            2 => _settings.Multipliers.Quality.Rating2,
            1 => _settings.Multipliers.Quality.Rating1,
            _ => _settings.Multipliers.Quality.Rating3 // По умолчанию средний
        };
    }

    /// <summary>
    /// Бонус за streak (чем дольше streak, тем больше бонус)
    /// </summary>
    private double CalculateStreakBonus(int currentStreak)
    {
        return currentStreak switch
        {
            >= 30 => _settings.Multipliers.StreakBonus.Days30Plus,  // Месяц подряд!
            >= 14 => _settings.Multipliers.StreakBonus.Days14Plus,  // Две недели
            >= 7 => _settings.Multipliers.StreakBonus.Days7Plus,    // Неделя
            _ => _settings.Multipliers.StreakBonus.Default          // Нет бонуса
        };
    }
    
    
    
    
    /// <summary>
    /// Генерирует мотивационное сообщение
    /// </summary>
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

        // Близко к новому уровню (менее 200 XP)
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
        // Активный streak (7+ дней)
        else if (userStats.CurrentStreak >= 7)
        {
            message = new MotivationalMessageDto
            {
                Message = $"Отличный streak! {userStats.CurrentStreak} дней подряд! Так держать!",
                Icon = "🔥",
                Type = "streak"
            };
        }
        // Обычная мотивация до уровня
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
    
    /// <summary>
    /// Добавляет XP пользователю и проверяет level up
    /// </summary>
    public async Task<ServiceResult<(bool leveledUp, int newLevel)>> AddXPToUserAsync(Guid userId, int xp)
    {
        var userStats = await _context.UserStatistics.FirstOrDefaultAsync(us => us.UserId == userId);
        
        if (userStats == null)
        {
            return ServiceResult<(bool, int)>.Failure("User statistics not found");
        }

        var oldLevel = userStats.Level;
        userStats.TotalXP += xp;

        // Проверяем повышение уровня
        var newLevel = CalculateLevelFromXP(userStats.TotalXP);
        var leveledUp = newLevel > oldLevel;

        if (leveledUp)
        {
            userStats.Level = newLevel;
        }

        await _context.SaveChangesAsync();

        return ServiceResult<(bool leveledUp, int newLevel)>.Success((leveledUp, newLevel));
    }

    /// <summary>
    /// Рассчитывает сколько XP нужно для достижения уровня
    /// Формула прогрессии: уровни становятся сложнее с ростом
    /// </summary>
    public int CalculateXPForLevel(int level)
    {
        if (level == 1) return 0;
        if (level <= 10) return 100 * level;  // Быстрый рост в начале
        if (level <= 25) return 100 * level + 50 * (level - 10);
        return 100 * level + 50 * 15 + 100 * (level - 25);
    }

    /// <summary>
    /// Рассчитывает уровень из общего количества XP
    /// </summary>
    private int CalculateLevelFromXP(int totalXP)
    {
        int level = 1;
        while (CalculateXPForLevel(level + 1) <= totalXP)
        {
            level++;
        }
        return level;
    }

    /// <summary>
    /// Обновляет streak пользователя
    /// </summary>
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
            // Уже занимался сегодня, streak не меняется
            streakIncreased = false;
        }
        else if (daysDifference == 1)
        {
            // Занимался вчера, streak продолжается
            userStats.CurrentStreak++;
            streakIncreased = true;
            
            if (userStats.CurrentStreak > userStats.BestStreak)
                userStats.BestStreak = userStats.CurrentStreak;
        }
        else
        {
            // Пропуск более 1 дня - streak обнуляется
            userStats.CurrentStreak = 1;
            streakIncreased = false;
        }

        userStats.LastStudyDate = today;
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Success(streakIncreased);
    }
}