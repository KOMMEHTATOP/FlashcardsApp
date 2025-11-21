using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.BLL.Mapping;
using FlashcardsApp.DAL;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.Constants;
using FlashcardsApp.Models.DTOs.Requests;
using FlashcardsApp.Models.DTOs.Responses;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.BLL.Implementations;

public class StudySettingsBL : IStudySettingsBL
{
    private readonly ApplicationDbContext _context;
    
    public StudySettingsBL(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<ServiceResult<ResultSettingsDto>> GetStudySettingsAsync(Guid userId)
    {
        var settings = await _context.StudySettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (settings != null)
        {
            return ServiceResult<ResultSettingsDto>.Success(settings.ToDto());
        }

        return ServiceResult<ResultSettingsDto>.Success(ResultSettingsDto.GetDefault());
    }

    public async Task<ServiceResult<ResultSettingsDto>> SaveStudySettingsAsync(Guid userId, CreateSettingsDto dto)
    {
        var settings = await _context.StudySettings
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (settings != null)
        {
            settings.MinRating = dto.MinRating;
            settings.MaxRating = dto.MaxRating;
            settings.StudyOrder = dto.StudyOrder;
            settings.CompletionThreshold = dto.CompletionThreshold;
            settings.ShuffleOnRepeat = dto.ShuffleOnRepeat;

            _context.StudySettings.Update(settings);
        }
        else
        {
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
    
    public async Task<ServiceResult<ResultSettingsDto>> ResetToDefaultAsync(Guid userId)
    {
        var settings = await _context.StudySettings
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (settings != null)
        {
            _context.StudySettings.Remove(settings);
            await _context.SaveChangesAsync();
        }

        return ServiceResult<ResultSettingsDto>.Success(ResultSettingsDto.GetDefault());
    }
}