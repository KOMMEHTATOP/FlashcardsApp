using FlashcardsApp.Data;
using FlashcardsApp.Mapping;
using FlashcardsApp.Models;
using FlashcardsAppContracts.DTOs.Requests;
using FlashcardsAppContracts.DTOs.Responses;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FlashcardsApp.Services;

public class CardService
{
    private readonly ApplicationDbContext _context;

    public CardService(ApplicationDbContext context)
    {
        _context = context;
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

        return ServiceResult<IEnumerable<ResultCardDto>>.Success(cards.Select(c => c.ToDto()));
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
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            return ServiceResult<ResultCardDto>.Failure("You already have a card with this question");
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

        card.Question = dto.Question;
        card.Answer = dto.Answer;
        card.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            return ServiceResult<ResultCardDto>.Failure("You already have a card with this question");
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
        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Success(true);
    }

    private bool IsUniqueConstraintViolation(DbUpdateException exception)
    {
        if (exception.InnerException is PostgresException postgresEx)
        {
            return postgresEx.SqlState == "23505";
        }
        return false;
    }
}