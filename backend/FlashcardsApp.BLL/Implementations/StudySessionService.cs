using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.DAL;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.Constants;
using FlashcardsApp.Models.DTOs.Study;
using FlashcardsApp.Models.DTOs.Study.Responses;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.BLL.Implementations;

public class StudySessionService : IStudySessionService
{
    private readonly ApplicationDbContext _context;
    private readonly IStudySettingsService _studySettingsService;

    public StudySessionService(ApplicationDbContext context, IStudySettingsService studySettingsService)
    {
        _context = context;
        _studySettingsService = studySettingsService;
    }

    /// <summary>
    /// Начать сессию изучения для конкретной группы
    /// </summary>
    public async Task<ServiceResult<ResultStudySessionDto>> StartSessionAsync(Guid userId, Guid groupId)
    {
        // Получаем глобальные настройки пользователя (или дефолтные)
        var settingsResult = await _studySettingsService.GetStudySettingsAsync(userId);

        if (!settingsResult.IsSuccess || settingsResult.Data == null)
        {
            return ServiceResult<ResultStudySessionDto>.Failure("Failed to get settings");
        }

        var settings = settingsResult.Data;

        // Получаем карточки группы
        var cards = await _context.Cards
            .Where(c => c.UserId == userId && c.GroupId == groupId)
            .ToListAsync();

        // Получаем последние оценки для всех карточек из StudyHistory
        var cardIds = cards.Select(c => c.CardId).ToList();
        
        var lastRatings = await _context.StudyHistory
            .Where(sh => sh.UserId == userId && cardIds.Contains(sh.CardId))
            .GroupBy(sh => sh.CardId)
            .Select(g => new 
            { 
                CardId = g.Key, 
                LastRating = g.OrderByDescending(sh => sh.StudiedAt)
                              .Select(sh => sh.Rating)
                              .FirstOrDefault() 
            })
            .ToDictionaryAsync(x => x.CardId, x => x.LastRating);

        // Фильтруем по диапазону оценок
        var filteredCards = cards.Where(card =>
        {
            var lastRating = lastRatings.GetValueOrDefault(card.CardId, 0);
            return lastRating >= settings.MinRating && lastRating <= settings.MaxRating;
        }).ToList();

        // Сортируем согласно настройкам
        List<Card> sortedCards = settings.StudyOrder switch
        {
            StudyOrder.CreatedDate => filteredCards.OrderBy(c => c.CreatedAt).ToList(),
            StudyOrder.Rating => filteredCards.OrderBy(c => 
                lastRatings.GetValueOrDefault(c.CardId, 0)
            ).ToList(),
            StudyOrder.Random => filteredCards.OrderBy(c => Guid.NewGuid()).ToList(),
            _ => filteredCards
        };

        // Преобразуем в DTO
        var studyCards = sortedCards.Select(card => new StudyCardDto
        {
            CardId = card.CardId,
            Question = card.Question,
            Answer = card.Answer,
            LastRating = lastRatings.GetValueOrDefault(card.CardId, 0)
        }).ToList();

        var response = new ResultStudySessionDto
        {
            Cards = studyCards
        };

        return ServiceResult<ResultStudySessionDto>.Success(response);
    }
}