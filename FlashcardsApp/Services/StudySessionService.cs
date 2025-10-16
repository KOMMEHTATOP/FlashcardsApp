using FlashcardsApp.Data;
using FlashcardsApp.Interfaces;
using FlashcardsApp.Models;
using FlashcardsAppContracts.Constants;
using FlashcardsAppContracts.DTOs.Responses;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.Services;

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

        // Получаем карточки группы с последней оценкой
        var cards = await _context.Cards
            .Where(c => c.UserId == userId && c.GroupId == groupId)
            .Include(c => c.Ratings)
            .ToListAsync();

        // Фильтруем по диапазону оценок
        var filteredCards = cards.Where(card =>
        {
            var lastRating = card.Ratings?
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefault()?.Rating ?? 0;

            return lastRating >= settings.MinRating && lastRating <= settings.MaxRating;
        }).ToList();

        // Сортируем согласно настройкам
        List<Card> sortedCards = settings.StudyOrder switch
        {
            StudyOrder.CreatedDate => filteredCards.OrderBy(c => c.CreatedAt).ToList(),
            StudyOrder.Rating => filteredCards.OrderBy(c =>
                c.Ratings?.OrderByDescending(r => r.CreatedAt).FirstOrDefault()?.Rating ?? 0
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
            LastRating = card.Ratings?
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefault()?.Rating ?? 0
        }).ToList();

        var response = new ResultStudySessionDto
        {
            Cards = studyCards
        };

        return ServiceResult<ResultStudySessionDto>.Success(response);
    }
}
