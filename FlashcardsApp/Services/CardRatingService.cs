using FlashcardsApp.Data;
using FlashcardsApp.Interfaces;
using FlashcardsApp.Mapping;
using FlashcardsApp.Models;
using FlashcardsAppContracts.DTOs.Requests;
using FlashcardsAppContracts.DTOs.Responses;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.Services;

public class CardRatingService : ICardRatingService
{
    private readonly ApplicationDbContext _context;

    public CardRatingService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<ServiceResult<IEnumerable<ResultCardRatingDto>>> GetCardRatingsAsync(Guid userId, Guid cardId)
    {
        var card = await _context.Cards
            .AsNoTracking()
            .Include(c => c.Ratings)
            .FirstOrDefaultAsync(c => c.CardId == cardId && c.UserId == userId);

        if (card == null)
        {
            return ServiceResult<IEnumerable<ResultCardRatingDto>>.Failure("Card not found or access denied");
        }

        var ratings = card.Ratings?.Select(r => r.ToDto()) ?? [];

        return ServiceResult<IEnumerable<ResultCardRatingDto>>.Success(ratings);
    }

    public async Task<ServiceResult<ResultCardRatingDto>> CreateCardRatingAsync(Guid cardId, Guid userId, CreateCardRatingDto dto)
    {
        if (dto.Rating is < 1 or > 5)
        {
            return ServiceResult<ResultCardRatingDto>.Failure("Rating must be between 1 and 5");
        }
        
        var card = await _context.Cards
            .FirstOrDefaultAsync(c => c.CardId == cardId && c.UserId == userId);

        if (card == null)
        {
            return ServiceResult<ResultCardRatingDto>.Failure("Card not found or access denied");
        }

        var newRating = new CardRating
        {
            Id = Guid.NewGuid(),
            CardId = cardId, 
            UserId = userId, 
            CreatedAt = DateTime.UtcNow, 
            Rating = dto.Rating,
        };

        _context.CardRatings.Add(newRating);
        await _context.SaveChangesAsync();
        return ServiceResult<ResultCardRatingDto>.Success(newRating.ToDto());
    }
    
    public async Task<ServiceResult<bool>> DeleteCardRatingsAsync(Guid cardId, Guid userId)
    {
        var card = await _context.Cards
            .Include(c => c.Ratings)
            .FirstOrDefaultAsync(c => c.CardId == cardId && c.UserId == userId);

        if (card == null)
        {
            return ServiceResult<bool>.Failure("Card not found");
        }

        if (card.Ratings != null && card.Ratings.Any())
        {
            _context.CardRatings.RemoveRange(card.Ratings);
            await _context.SaveChangesAsync();
        }

        return ServiceResult<bool>.Success(true);
    }
}
