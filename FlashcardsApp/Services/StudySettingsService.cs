using FlashcardsApp.Data;
using FlashcardsApp.Mapping;
using FlashcardsApp.Models;
using FlashcardsAppContracts.Constants;
using FlashcardsAppContracts.DTOs.Requests;
using FlashcardsAppContracts.DTOs.Responses;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.Services;

public class StudySettingsService
{
    private readonly ApplicationDbContext _context;
    
    public StudySettingsService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Получить глобальные настройки пользователя
    /// </summary>
    public async Task<ServiceResult<ResultSettingsDto>> GetStudySettingsAsync(Guid userId)
    {
        var settings = await _context.StudySettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (settings != null)
        {
            return ServiceResult<ResultSettingsDto>.Success(settings.ToDto());
        }

        // Возвращаем дефолтные настройки (не сохраняем в БД)
        var defaultSettings = new ResultSettingsDto
        {
            StudyOrder = StudyOrder.Random,
            MinRating = 0,
            MaxRating = 5,
            CompletionThreshold = 5,
            ShuffleOnRepeat = true
        };

        return ServiceResult<ResultSettingsDto>.Success(defaultSettings);
    }
    
    /// <summary>
    /// Сохранить глобальные настройки пользователя
    /// </summary>
    public async Task<ServiceResult<ResultSettingsDto>> SaveStudySettingsAsync(Guid userId, CreateSettingsDto dto)
    {
        var settings = await _context.StudySettings
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (settings != null)
        {
            // Обновляем существующие настройки
            settings.MinRating = dto.MinRating;
            settings.MaxRating = dto.MaxRating;
            settings.StudyOrder = dto.StudyOrder;
            settings.CompletionThreshold = dto.CompletionThreshold;
            settings.ShuffleOnRepeat = dto.ShuffleOnRepeat;

            _context.StudySettings.Update(settings);
        }
        else
        {
            // Создаем новые настройки
            settings = new StudySettings
            {
                StudySettingsId = Guid.NewGuid(),
                UserId = userId,
                MinRating = dto.MinRating,
                MaxRating = dto.MaxRating,
                StudyOrder = dto.StudyOrder,
                CompletionThreshold = dto.CompletionThreshold,
                ShuffleOnRepeat = dto.ShuffleOnRepeat
            };
            _context.StudySettings.Add(settings);
        }

        await _context.SaveChangesAsync();
        return ServiceResult<ResultSettingsDto>.Success(settings.ToDto());
    }
    
    /// <summary>
    /// Сбросить настройки к дефолтным (удалить пользовательские)
    /// </summary>
    public async Task<ServiceResult<ResultSettingsDto>> ResetToDefaultAsync(Guid userId)
    {
        var settings = await _context.StudySettings
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (settings != null)
        {
            _context.StudySettings.Remove(settings);
            await _context.SaveChangesAsync();
        }

        // Возвращаем дефолтные настройки
        var defaultSettings = new ResultSettingsDto
        {
            StudyOrder = StudyOrder.Random,
            MinRating = 0,
            MaxRating = 5,
            CompletionThreshold = 5,
            ShuffleOnRepeat = true
        };

        return ServiceResult<ResultSettingsDto>.Success(defaultSettings);
    }
}