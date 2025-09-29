using FlashcardsApp.Data;
using FlashcardsApp.Models;
using FlashcardsAppContracts.Constants;
using FlashcardsAppContracts.DTOs.Responses;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.Services;

public class StudySessionService
{
    private readonly ApplicationDbContext _context;
    private readonly StudySettingsService _studySettingsService;

    public StudySessionService(ApplicationDbContext context, StudySettingsService studySettingsService)
    {
        _context = context;
        _studySettingsService = studySettingsService;
    }

public async Task<ServiceResult<ResultStudySessionDto>> StartSessionAsync(Guid userId, Guid groupId, bool useDefaultSettings = false)
{
    Console.WriteLine($"=== START SESSION: useDefaultSettings={useDefaultSettings} ===");

    ServiceResult<ResultSettingsDto> settingsResult;
    
    if (useDefaultSettings)
    {
        settingsResult = await _studySettingsService.GetDefaultSettingsAsync(userId, groupId);
    }
    else
    {
        settingsResult = await _studySettingsService.GetStudySettingsAsync(userId, groupId);
    }

    if (!settingsResult.IsSuccess || settingsResult.Data == null)
    {
        return ServiceResult<ResultStudySessionDto>.Failure("Failed to get settings");
    }

    var settings = settingsResult.Data;
    Console.WriteLine($"Settings loaded: MinRating={settings.MinRating}, MaxRating={settings.MaxRating}, GroupId={settings.GroupId}");

    var cards = await _context.Cards
        .Where(c => c.UserId == userId && c.GroupId == groupId)
        .Include(c => c.Ratings!.OrderByDescending(r => r.CreatedAt).Take(1))
        .ToListAsync();

    Console.WriteLine($"Total cards in group: {cards.Count}");

    var filtredCards = cards.Where(card =>
    {
        var lastRating = card.Ratings?.FirstOrDefault()?.Rating ?? 0;
        return lastRating >= settings.MinRating && lastRating <= settings.MaxRating;
    }).ToList();
    
    Console.WriteLine($"Filtered cards: {filtredCards.Count}");


    List<Card> sortedCards;

    if (settings.StudyOrder == StudyOrder.CreatedDate)
    {
        sortedCards = filtredCards.OrderBy(c => c.CreatedAt).ToList();
    }
    else if (settings.StudyOrder == StudyOrder.Rating)
    {
        sortedCards = filtredCards.OrderBy(c =>
            c.Ratings?.FirstOrDefault()?.Rating ?? 0).ToList();
    }
    else if (settings.StudyOrder == StudyOrder.Random)
    {
        var random = new Random();
        sortedCards = filtredCards.OrderBy(c => random.Next()).ToList();
    }
    else
    {
        sortedCards = filtredCards;
    }

    var studyCards = sortedCards.Select(card => new StudyCardDto
    {
        CardId = card.CardId,
        Question = card.Question,
        Answer = card.Answer,
        LastRating = card.Ratings?.FirstOrDefault()?.Rating ?? 0
    }).ToList();

    var response = new ResultStudySessionDto
    {
        Cards = studyCards
    };
    
    return ServiceResult<ResultStudySessionDto>.Success(response);
}


}
