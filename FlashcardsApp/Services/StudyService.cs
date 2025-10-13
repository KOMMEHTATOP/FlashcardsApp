using FlashcardsApp.Data;
using FlashcardsApp.Models;
using FlashcardsAppContracts.DTOs.Requests;
using FlashcardsAppContracts.DTOs.Responses;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.Services;

public class StudyService
{
    private readonly ApplicationDbContext _context;
    private readonly GamificationService _gamificationService;

    public StudyService(ApplicationDbContext context, GamificationService gamificationService)
    {
        _context = context;
        _gamificationService = gamificationService;
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

            // 5. Получаем актуальную статистику пользователя
            var userStats = await _context.UserStatistics
                .AsNoTracking()
                .FirstOrDefaultAsync(us => us.UserId == userId);

            if (userStats == null)
            {
                await transaction.RollbackAsync();
                return ServiceResult<StudyRewardDto>.Failure("User statistics not found");
            }

            await transaction.CommitAsync();

            // 6. Формируем ответ
            var reward = new StudyRewardDto
            {
                XPEarned = xpEarned,
                TotalXP = userStats.TotalXP,
                CurrentLevel = userStats.Level,
                LeveledUp = leveledUp,
                NewLevel = newLevel,
                StreakIncreased = streakIncreased,
                CurrentStreak = userStats.CurrentStreak
            };

            return ServiceResult<StudyRewardDto>.Success(reward);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
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