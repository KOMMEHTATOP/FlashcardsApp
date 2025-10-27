using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.BLL.Interfaces.Achievements;
using FlashcardsApp.DAL.Data;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.DTOs.Achievements.Responses;
using FlashcardsApp.Models.DTOs.Study.Requests;
using FlashcardsApp.Models.DTOs.Study.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FlashcardsApp.BLL.Implementations;

public class StudyService : IStudyService
{
    private readonly ApplicationDbContext _context;
    private readonly IGamificationService _gamificationService;
    private readonly IAchievementService _achievementService;
    private readonly ILogger<StudyService> _logger;

    public StudyService(
        ApplicationDbContext context, 
        IGamificationService gamificationService,
        IAchievementService achievementService,
        ILogger<StudyService> logger)
    {
        _context = context;
        _gamificationService = gamificationService;
        _achievementService = achievementService;
        _logger = logger;
    }

    /// <summary>
    /// Записывает сессию изучения карточки и начисляет награды
    /// </summary>
    public async Task<ServiceResult<StudyRewardDto>> RecordStudySessionAsync(Guid userId, RecordStudyDto dto)
    {
        // Проверяем что карточка принадлежит пользователю
        var cardExists = await _context.Cards.AnyAsync(c => c.CardId == dto.CardId && c.UserId == userId);
        
        if (!cardExists)
        {
            return ServiceResult<StudyRewardDto>.Failure("Card not found or access denied");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            // 1. Рассчитываем XP за карточку
            var xpEarned = await _gamificationService.CalculateXPForCardAsync(userId, dto.CardId, dto.Rating);

            // 2. Сохраняем в историю изучения
            var studyHistory = new StudyHistory
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CardId = dto.CardId,
                Rating = dto.Rating,
                XPEarned = xpEarned,
                StudiedAt = DateTime.UtcNow
            };
            _context.StudyHistory.Add(studyHistory);

            // 3. Добавляем XP пользователю и проверяем level up
            var addXpResult = await _gamificationService.AddXPToUserAsync(userId, xpEarned);
            if (!addXpResult.IsSuccess)
            {
                await transaction.RollbackAsync();
                return ServiceResult<StudyRewardDto>.Failure(addXpResult.Errors.ToArray());
            }

            var (leveledUp, newLevel) = addXpResult.Data;

            // 4. Обновляем streak
            var streakResult = await _gamificationService.UpdateStreakAsync(userId);
            if (!streakResult.IsSuccess)
            {
                await transaction.RollbackAsync();
                return ServiceResult<StudyRewardDto>.Failure(streakResult.Errors.ToArray());
            }

            var streakIncreased = streakResult.Data;

            // 5. Обновляем статистику карточек
            var userStats = await _context.UserStatistics
                .FirstOrDefaultAsync(us => us.UserId == userId);

            if (userStats == null)
            {
                await transaction.RollbackAsync();
                return ServiceResult<StudyRewardDto>.Failure("User statistics not found");
            }

            userStats.TotalCardsStudied++;
            
            // Обновляем Perfect Ratings Streak
            if (dto.Rating == 5)
            {
                userStats.PerfectRatingsStreak++;
            }
            else
            {
                userStats.PerfectRatingsStreak = 0;
            }

            await _context.SaveChangesAsync();

            // 6. Проверяем достижения (после сохранения статистики)
            var achievementResult = await _achievementService.CheckAndUnlockAchievementsAsync(userId);
            
            var newAchievements = achievementResult.IsSuccess 
                ? achievementResult.Data 
                : new List<AchievementDto>();

            if (newAchievements.Any())
            {
                _logger.LogInformation(
                    "{Count} new achievements unlocked for user {UserId}: {Achievements}",
                    newAchievements.Count, 
                    userId, 
                    string.Join(", ", newAchievements.Select(a => a.Name)));
            }

            // 7. Получаем актуальную статистику пользователя (после всех обновлений)
            var updatedStats = await _context.UserStatistics
                .AsNoTracking()
                .FirstOrDefaultAsync(us => us.UserId == userId);

            if (updatedStats == null)
            {
                await transaction.RollbackAsync();
                return ServiceResult<StudyRewardDto>.Failure("User statistics not found");
            }

            // 8. Рассчитываем прогресс до следующего уровня
            var xpForCurrentLevel = _gamificationService.CalculateXPForLevel(updatedStats.Level);
            var xpForNextLevel = _gamificationService.CalculateXPForLevel(updatedStats.Level + 1);
            
            var currentLevelXP = updatedStats.TotalXP - xpForCurrentLevel;
            var xpNeededForLevel = xpForNextLevel - xpForCurrentLevel;
            var xpToNextLevel = xpForNextLevel - updatedStats.TotalXP;

            await transaction.CommitAsync();

            // 9. Формируем ответ
            var reward = new StudyRewardDto
            {
                // XP и уровень
                XPEarned = xpEarned,
                TotalXP = updatedStats.TotalXP,
                CurrentLevel = updatedStats.Level,
                LeveledUp = leveledUp,
                
                // Прогресс до следующего уровня
                CurrentLevelXP = currentLevelXP,
                XPForNextLevel = xpNeededForLevel,
                XPToNextLevel = xpToNextLevel,
                
                // Streak
                StreakIncreased = streakIncreased,
                CurrentStreak = updatedStats.CurrentStreak,
                
                // Достижения
                NewAchievements = newAchievements.Select(a => new AchievementUnlockedDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    IconUrl = a.IconUrl,
                    Rarity = a.Rarity
                }).ToList()
            };

            return ServiceResult<StudyRewardDto>.Success(reward);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error recording study session for user {UserId}", userId);
            return ServiceResult<StudyRewardDto>.Failure($"Error recording study session: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Получить историю изучения
    /// </summary>
    public async Task<ServiceResult<IEnumerable<StudyHistoryDto>>> GetStudyHistoryAsync(Guid userId, int? limit = 50)
    {
        var history = await _context.StudyHistory
            .AsNoTracking()
            .Where(sh => sh.UserId == userId)
            .OrderByDescending(sh => sh.StudiedAt)
            .Take(limit ?? 50)
            .Include(sh => sh.Card)
            .Select(sh => new StudyHistoryDto
            {
                Id = sh.Id,
                CardId = sh.CardId,
                CardQuestion = sh.Card != null ? sh.Card.Question : "",
                Rating = sh.Rating,
                XPEarned = sh.XPEarned,
                StudiedAt = sh.StudiedAt
            })
            .ToListAsync();

        return ServiceResult<IEnumerable<StudyHistoryDto>>.Success(history);
    }
}