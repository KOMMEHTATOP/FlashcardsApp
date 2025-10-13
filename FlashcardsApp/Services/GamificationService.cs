using FlashcardsApp.Data;
using FlashcardsAppContracts.DTOs.Responses;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.Services;

public class GamificationService
{
    private readonly ApplicationDbContext _context;

    // Константы для баланса (потом вынесешь в appsettings.json)
    private const int BASE_XP = 10;

    public GamificationService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Рассчитывает XP за изученную карточку
    /// </summary>
    public async Task<int> CalculateXPForCardAsync(Guid userId, Guid cardId, int rating)
    {
        // 1. Рассчитываем сложность карточки (на основе истории)
        var difficultyMultiplier = await CalculateDifficultyMultiplierAsync(userId, cardId);

        // 2. Рассчитываем множитель качества (оценка пользователя)
        var qualityMultiplier = CalculateQualityMultiplier(rating);

        // 3. Получаем streak пользователя для бонуса
        var userStats = await _context.UserStatistics
            .AsNoTracking()
            .FirstOrDefaultAsync(us => us.UserId == userId);
        
        var streakBonus = CalculateStreakBonus(userStats?.CurrentStreak ?? 0);

        // 4. Итоговый расчет
        var xp = (int)(BASE_XP * difficultyMultiplier * qualityMultiplier * streakBonus);

        return xp;
    }

    /// <summary>
    /// Рассчитывает множитель сложности на основе истории изучения
    /// </summary>
    private async Task<double> CalculateDifficultyMultiplierAsync(Guid userId, Guid cardId)
    {
        // Берем последние 5 оценок этой карточки пользователем
        var recentRatings = await _context.StudyHistory
            .Where(sh => sh.UserId == userId && sh.CardId == cardId)
            .OrderByDescending(sh => sh.StudiedAt)
            .Take(5)
            .Select(sh => sh.Rating)
            .ToListAsync();

        if (!recentRatings.Any())
            return 1.0; // Средняя сложность для новой карточки

        var averageRating = recentRatings.Average();

        // Чем ниже средняя оценка, тем сложнее карточка → больше XP
        return averageRating switch
        {
            >= 4.0 => 0.8,   // Легкая карточка
            >= 2.5 => 1.0,   // Средняя
            _ => 1.5         // Сложная
        };
    }

    /// <summary>
    /// Множитель на основе оценки (1-5)
    /// </summary>
    private double CalculateQualityMultiplier(int rating)
    {
        return rating switch
        {
            5 => 1.5,  // Отлично знаю
            4 => 1.2,
            3 => 1.0,  // Нормально
            2 => 0.7,
            1 => 0.5,  // Совсем не знаю
            _ => 1.0
        };
    }

    /// <summary>
    /// Бонус за streak (чем дольше streak, тем больше бонус)
    /// </summary>
    private double CalculateStreakBonus(int currentStreak)
    {
        return currentStreak switch
        {
            >= 30 => 1.5,   // Месяц подряд!
            >= 14 => 1.25,  // Две недели
            >= 7 => 1.1,    // Неделя
            _ => 1.0        // Нет бонуса
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
            var cardsNeeded = (int)Math.Ceiling(xpNeeded / 10.0);
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
    public async Task<ServiceResult<(bool leveledUp, int? newLevel)>> AddXPToUserAsync(Guid userId, int xp)
    {
        var userStats = await _context.UserStatistics.FirstOrDefaultAsync(us => us.UserId == userId);
        
        if (userStats == null)
        {
            return ServiceResult<(bool, int?)>.Failure("User statistics not found");
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

        return ServiceResult<(bool leveledUp, int? newLevel)>.Success((leveledUp, leveledUp ? newLevel : null));
    }

    /// <summary>
    /// Рассчитывает сколько XP нужно для достижения уровня
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