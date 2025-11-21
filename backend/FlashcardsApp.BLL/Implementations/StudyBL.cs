using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.BLL.Interfaces.Achievements;
using FlashcardsApp.DAL;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.DTOs.Achievements.Responses;
using FlashcardsApp.Models.DTOs.Study.Requests;
using FlashcardsApp.Models.DTOs.Study.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FlashcardsApp.BLL.Implementations;

public class StudyBL : IStudyBL
{
    private readonly ApplicationDbContext _context;
    private readonly IGamificationBL _gamificationBL;
    private readonly IAchievementBL _achievementBL;
    private readonly ILogger<StudyBL> _logger;

    public StudyBL(
        ApplicationDbContext context, 
        IGamificationBL gamificationBL,
        IAchievementBL achievementBL,
        ILogger<StudyBL> logger)
    {
        _context = context;
        _gamificationBL = gamificationBL;
        _achievementBL = achievementBL;
        _logger = logger;
    }
    
    public async Task<ServiceResult<StudyRewardDto>> RecordStudySessionAsync(Guid userId, RecordStudyDto dto)
    {
        var cardExists = await _context.Cards.AnyAsync(c => c.CardId == dto.CardId);
        
        if (!cardExists)
        {
            return ServiceResult<StudyRewardDto>.Failure("Card not found or access denied");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            var xpEarned = await _gamificationBL.CalculateXPForCardAsync(userId, dto.CardId, dto.Rating);

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

            var addXpResult = await _gamificationBL.AddXPToUserAsync(userId, xpEarned);
            if (!addXpResult.IsSuccess)
            {
                await transaction.RollbackAsync();
                return ServiceResult<StudyRewardDto>.Failure(addXpResult.Errors.ToArray());
            }

            var (leveledUp, newLevel) = addXpResult.Data;
            var streakResult = await _gamificationBL.UpdateStreakAsync(userId);
            
            if (!streakResult.IsSuccess)
            {
                await transaction.RollbackAsync();
                return ServiceResult<StudyRewardDto>.Failure(streakResult.Errors.ToArray());
            }

            var streakIncreased = streakResult.Data;
            var userStats = await _context.UserStatistics
                .FirstOrDefaultAsync(us => us.UserId == userId);

            if (userStats == null)
            {
                await transaction.RollbackAsync();
                return ServiceResult<StudyRewardDto>.Failure("User statistics not found");
            }

            userStats.TotalCardsStudied++;
            
            if (dto.Rating == 5)
            {
                userStats.PerfectRatingsStreak++;
            }
            else
            {
                userStats.PerfectRatingsStreak = 0;
            }

            await _context.SaveChangesAsync();

            var achievementResult = await _achievementBL.CheckAndUnlockAchievementsAsync(userId);
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

            var updatedStats = await _context.UserStatistics
                .AsNoTracking()
                .FirstOrDefaultAsync(us => us.UserId == userId);

            if (updatedStats == null)
            {
                await transaction.RollbackAsync();
                return ServiceResult<StudyRewardDto>.Failure("User statistics not found");
            }

            var xpForCurrentLevel = _gamificationBL.CalculateXPForLevel(updatedStats.Level);
            var xpForNextLevel = _gamificationBL.CalculateXPForLevel(updatedStats.Level + 1);
            
            var currentLevelXP = updatedStats.TotalXP - xpForCurrentLevel;
            var xpNeededForLevel = xpForNextLevel - xpForCurrentLevel;
            var xpToNextLevel = xpForNextLevel - updatedStats.TotalXP;

            await transaction.CommitAsync();

            var reward = new StudyRewardDto
            {
                XPEarned = xpEarned,
                TotalXP = updatedStats.TotalXP,
                CurrentLevel = updatedStats.Level,
                LeveledUp = leveledUp,
                
                CurrentLevelXP = currentLevelXP,
                XPForNextLevel = xpNeededForLevel,
                XPToNextLevel = xpToNextLevel,
                
                StreakIncreased = streakIncreased,
                CurrentStreak = updatedStats.CurrentStreak,
                
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

    public async Task<ServiceResult<IEnumerable<StudyHistoryDto>>> GetStudyHistoryAsync(Guid userId, int? limit = 50)
    {
        var history = await _context.StudyHistory
            .AsNoTracking()
            .Where(sh => sh.UserId == userId)
            .OrderByDescending(sh => sh.StudiedAt)
            .Take(limit ?? 50)
            .Include(sh => sh.Card)
            .ThenInclude(c => c.Group)  
            .Select(sh => new StudyHistoryDto
            {
                Id = sh.Id,
                CardId = sh.CardId,
                CardQuestion = sh.Card != null ? sh.Card.Question : "",
                Rating = sh.Rating,
                XPEarned = sh.XPEarned,
                StudiedAt = sh.StudiedAt,
                GroupName = sh.Card != null && sh.Card.Group != null ? sh.Card.Group.GroupName : "",
                GroupColor = sh.Card != null && sh.Card.Group != null ? sh.Card.Group.GroupColor : "from-gray-500 to-gray-600"
            })
            .ToListAsync();

        return ServiceResult<IEnumerable<StudyHistoryDto>>.Success(history);
    }
    
}