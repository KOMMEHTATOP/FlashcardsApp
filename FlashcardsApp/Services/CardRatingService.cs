using FlashcardsApp.Data;
using FlashcardsApp.Mapping;
using FlashcardsApp.Models;
using FlashcardsApp.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.Services;

public class CardRatingService
{
    private readonly ApplicationDbContext _context;

    public CardRatingService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<IEnumerable<ResultCardRating>>> GetAllCardRatings(Guid userId)
    {
        var cardRatings = await _context.CardRatings
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();

        return ServiceResult<IEnumerable<ResultCardRating>>.Success(cardRatings.Select(c => c.ToDto()));
    }

    public async Task<ServiceResult<IEnumerable<ResultCardRating>>> GetCardRating(Guid userId, Guid cardId)
    {
        var card = await _context.Cards
            .AsNoTracking()
            .Include(c => c.Ratings)
            .FirstOrDefaultAsync(c => c.CardId == cardId && c.UserId == userId);

        if (card == null)
        {
            return ServiceResult<IEnumerable<ResultCardRating>>.Failure("Card not found or access denied");
        }

        var ratings = card.Ratings?.Select(r => r.ToDto()) ?? [];

        return ServiceResult<IEnumerable<ResultCardRating>>.Success(ratings);
    }

    public async Task<ServiceResult<ResultCardRating>> CreateCardRating(Guid cardId, Guid userId, CreateCardRatingDto dto)
    {
        if (dto.Rating is < 1 or > 5)
        {
            return ServiceResult<ResultCardRating>.Failure("Rating must be between 1 and 5");
        }
        
        var card = await _context.Cards
            .FirstOrDefaultAsync(c => c.CardId == cardId && c.UserId == userId);

        if (card == null)
        {
            return ServiceResult<ResultCardRating>.Failure("Card not found or access denied");
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
        return ServiceResult<ResultCardRating>.Success(newRating.ToDto());
    }
}
