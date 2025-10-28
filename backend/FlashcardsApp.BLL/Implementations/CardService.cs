using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.BLL.Interfaces.Achievements;
using FlashcardsApp.BLL.Mapping;
using FlashcardsApp.DAL;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.DTOs.Cards.Requests;
using FlashcardsApp.Models.DTOs.Cards.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FlashcardsApp.BLL.Implementations;

public class CardService : ICardService
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
            // Получаем последние оценки из StudyHistory
            var cardsWithRating = await query
                .Select(c => new
                {
                    Card = c,
                    LastRating = _context.StudyHistory
                        .Where(sh => sh.CardId == c.CardId && sh.UserId == userId)
                        .OrderByDescending(sh => sh.StudiedAt)
                        .Select(sh => (int?)sh.Rating)
                        .FirstOrDefault()
                })
                .ToListAsync();

            // Фильтруем: карточки без оценок или с оценкой <= targetRating
            var filteredCards = cardsWithRating
                .Where(x => !x.LastRating.HasValue || x.LastRating.Value <= targetRating.Value)
                .Select(x => x.Card)
                .OrderBy(card => card.CreatedAt)
                .ToList();

            return ServiceResult<IEnumerable<ResultCardDto>>.Success(filteredCards.Select(c => c.ToDto()));
        }

        var cards = await query.OrderBy(card => card.CreatedAt).ToListAsync();
        return ServiceResult<IEnumerable<ResultCardDto>>.Success(cards.Select(c => c.ToDto()));
    }

    public async Task<ServiceResult<IEnumerable<ResultCardDto>>> GetCardsByGroupAsync(Guid groupId, Guid userId)
    {
        var cards = await _context.Cards
            .AsNoTracking()
            .Where(c => c.GroupId == groupId && c.UserId == userId)
            .OrderBy(c => c.CreatedAt)
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

        // Получаем последние оценки для карточек из StudyHistory
        var cardIds = cards.Select(c => c.CardId).ToList();
        var lastRatings = await _context.StudyHistory
            .Where(sh => sh.UserId == userId && cardIds.Contains(sh.CardId))
            .GroupBy(sh => sh.CardId)
            .Select(g => new
            {
                CardId = g.Key,
                LastRating = g.OrderByDescending(sh => sh.StudiedAt).First().Rating
            })
            .ToDictionaryAsync(x => x.CardId, x => x.LastRating);

        var result = cards.Select(c => new ResultCardDto
        {
            CardId = c.CardId,
            GroupId = c.GroupId,
            Question = c.Question,
            Answer = c.Answer,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
            LastRating = lastRatings.GetValueOrDefault(c.CardId, 0)
        }).ToList();

        return ServiceResult<IEnumerable<ResultCardDto>>.Success(result);
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