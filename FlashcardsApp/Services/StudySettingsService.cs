using FlashcardsApp.Data;
using FlashcardsApp.Mapping;
using FlashcardsApp.Models;
using FlashcardsAppContracts.Constants;
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

    public async Task<ServiceResult<ResultSettingsDto>> GetStudySettingsAsync(Guid userId, Guid? groupId)
    {
        var currentSettings = await _context.StudySettings
            .Where(s => s.UserId == userId && s.GroupId == groupId)
            .FirstOrDefaultAsync();

        if (currentSettings == null)
        {
            currentSettings = new StudySettings()
            {
                StudySettingsId = Guid.NewGuid(),
                UserId = userId,
                GroupId = groupId,
                StudyOrder = StudyOrder.Random,
                MinRating = 0,
                MaxRating = 5,
                PresetName = "Default",
            };

            _context.StudySettings.Add(currentSettings);
            await _context.SaveChangesAsync();
        }
        
        var result = currentSettings.ToDto();
        
        return ServiceResult<ResultSettingsDto>.Success(result);
    }
}
