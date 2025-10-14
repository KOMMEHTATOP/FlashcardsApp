using FlashcardsApp.Data;
using FlashcardsApp.Interfaces.Achievements;
using FlashcardsApp.Mapping;
using FlashcardsApp.Models;
using FlashcardsAppContracts.DTOs.Requests;
using FlashcardsAppContracts.DTOs.Responses;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.Services;

public class CardService
{
    private readonly ApplicationDbContext _context;
    private readonly IAchievementService _achievementService;
    private readonly ILogger<CardService> _logger;

    public CardService(
        ApplicationDbContext context,
        IAchievementService achievementService,
        ILogger<CardService> logger)
    {
        _context = context;
        _achievementService = achievementService;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<ResultCardDto>>> GetAllCardsAsync(Guid userId, int? targetRating)
    {
        var query = _context.Cards
            .AsNoTracking()
            .Where(card => card.UserId == userId);

        if (targetRating.HasValue)
        {
            query = query.Where(c =>
                !_context.CardRatings.Any(r => r.CardId == c.CardId) ||
                _context.CardRatings
                    .Where(r => r.CardId == c.CardId)
                    .OrderByDescending(r => r.CreatedAt)
                    .First().Rating <= targetRating.Value);
        }

        var cards = await query.OrderBy(card => card.CreatedAt).ToListAsync();
        return ServiceResult<IEnumerable<ResultCardDto>>.Success(cards.Select(c => c.ToDto()));
    }

    public async Task<ServiceResult<IEnumerable<ResultCardDto>>> GetCardsByGroupAsync(Guid groupId, Guid userId)
    {
        var cards = await _context.Cards
            .AsNoTracking()
            .Where(c => c.GroupId == groupId && c.UserId == userId)
            .Include(c => c.Ratings) 
            .OrderBy(c => c.CreatedAt)
            .Select(c => new ResultCardDto
            {
                CardId = c.CardId,
                GroupId = c.GroupId,
                Question = c.Question,
                Answer = c.Answer,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                LastRating = c.Ratings != null && c.Ratings.Any()
                    ? c.Ratings.OrderByDescending(r => r.CreatedAt).First().Rating
                    : 0
            })
            .ToListAsync();

        if (!cards.Any())
        {
            var groupExists = await _context.Groups
                .AnyAsync(g => g.Id == groupId && g.UserId == userId);

            if (!groupExists)
            {
                return ServiceResult<IEnumerable<ResultCardDto>>.Failure("Group not found or access denied");
            }
        }

        return ServiceResult<IEnumerable<ResultCardDto>>.Success(cards);
    }
    
    public async Task<ServiceResult<ResultCardDto>> GetCardAsync(Guid cardId, Guid userId)
    {
        var card = await _context.Cards
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CardId == cardId && c.UserId == userId);

        if (card == null)
        {
            return ServiceResult<ResultCardDto>.Failure("Card not found or access denied");
        }

        return ServiceResult<ResultCardDto>.Success(card.ToDto());
    }

    public async Task<ServiceResult<ResultCardDto>> CreateCardAsync(Guid userId, Guid groupId, CreateCardDto dto)
    {
        // ============================================
        // ИСПРАВЛЕНИЕ 1: Валидация ДО сохранения
        // ============================================
        
        // Проверяем существование карточки с таким вопросом у пользователя
        var cardExists = await _context.Cards
            .AsNoTracking()
            .AnyAsync(c => c.UserId == userId && c.Question == dto.Question);

        if (cardExists)
        {
            _logger.LogWarning(
                "User {UserId} attempted to create duplicate card with question: {Question}", 
                userId, 
                dto.Question);
            return ServiceResult<ResultCardDto>.Failure("You already have a card with this question");
        }

        // Создаем карточку
        var card = new Card
        {
            CardId = Guid.NewGuid(),
            UserId = userId,
            GroupId = groupId,
            Question = dto.Question,
            Answer = dto.Answer,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Cards.Add(card);
        
        try
        {
            await _context.SaveChangesAsync();
            
            _logger.LogInformation(
                "Card {CardId} created successfully for user {UserId}", 
                card.CardId, 
                userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Error saving card for user {UserId}", 
                userId);
            return ServiceResult<ResultCardDto>.Failure("Failed to create card");
        }

        // ============================================
        // ИСПРАВЛЕНИЕ 2: Обновляем TotalCardsCreated
        // ============================================
        
        var userStats = await _context.UserStatistics
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (userStats != null)
        {
            userStats.TotalCardsCreated++;
            
            _logger.LogInformation(
                "User {UserId} TotalCardsCreated updated: {Total}", 
                userId, 
                userStats.TotalCardsCreated);

            await _context.SaveChangesAsync();
            
            // ============================================
            // ИСПРАВЛЕНИЕ 3: Проверяем достижения
            // ============================================
            
            await _achievementService.CheckAndUnlockAchievementsAsync(userId);
        }
        else
        {
            _logger.LogWarning(
                "UserStatistics not found for user {UserId} after card creation", 
                userId);
        }

        return ServiceResult<ResultCardDto>.Success(card.ToDto());
    }

    public async Task<ServiceResult<ResultCardDto>> UpdateCardAsync(Guid cardId, Guid userId, CreateCardDto dto)
    {
        var card = await _context.Cards
            .FirstOrDefaultAsync(c => c.CardId == cardId && c.UserId == userId);

        if (card == null)
        {
            return ServiceResult<ResultCardDto>.Failure("Card not found or access denied");
        }

        // ============================================
        // ИСПРАВЛЕНИЕ: Валидация ДО сохранения
        // ============================================
        
        // Проверяем уникальность только если вопрос изменился
        if (card.Question != dto.Question)
        {
            var questionExists = await _context.Cards
                .AsNoTracking()
                .AnyAsync(c => 
                    c.UserId == userId && 
                    c.Question == dto.Question && 
                    c.CardId != cardId);

            if (questionExists)
            {
                _logger.LogWarning(
                    "User {UserId} attempted to update card {CardId} with duplicate question: {Question}", 
                    userId, 
                    cardId, 
                    dto.Question);
                return ServiceResult<ResultCardDto>.Failure("You already have a card with this question");
            }
        }

        card.Question = dto.Question;
        card.Answer = dto.Answer;
        card.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
            
            _logger.LogInformation(
                "Card {CardId} updated successfully for user {UserId}", 
                cardId, 
                userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Error updating card {CardId} for user {UserId}", 
                cardId, 
                userId);
            return ServiceResult<ResultCardDto>.Failure("Failed to update card");
        }

        return ServiceResult<ResultCardDto>.Success(card.ToDto());
    }

    public async Task<ServiceResult<bool>> DeleteCardAsync(Guid cardId, Guid userId)
    {
        var card = await _context.Cards
            .FirstOrDefaultAsync(c => c.CardId == cardId && c.UserId == userId);

        if (card == null)
        {
            return ServiceResult<bool>.Failure("Card not found or access denied");
        }

        _context.Cards.Remove(card);
        
        try
        {
            await _context.SaveChangesAsync();
            
            _logger.LogInformation(
                "Card {CardId} deleted successfully for user {UserId}", 
                cardId, 
                userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Error deleting card {CardId} for user {UserId}", 
                cardId, 
                userId);
            return ServiceResult<bool>.Failure("Failed to delete card");
        }

        return ServiceResult<bool>.Success(true);
    }
}