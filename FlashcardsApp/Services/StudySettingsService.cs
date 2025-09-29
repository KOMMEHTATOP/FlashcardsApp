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

    public async Task<ServiceResult<ResultSettingsDto>> GetStudySettingsAsync(Guid userId, Guid? groupId)
    {
        // 1. Ищем UserPreset для конкретной группы
        var userSettings = await _context.StudySettings
            .Where(s => s.UserId == userId && s.GroupId == groupId && s.PresetName == "UserPreset")
            .FirstOrDefaultAsync();

        if (userSettings != null)
        {
            Console.WriteLine($"Loaded UserPreset for GroupId={groupId}");
            return ServiceResult<ResultSettingsDto>.Success(userSettings.ToDto());
        }

        // 2. Ищем глобальные UserPreset (GroupId = NULL)
        var globalUserSettings = await _context.StudySettings
            .Where(s => s.UserId == userId && s.GroupId == null && s.PresetName == "UserPreset")
            .FirstOrDefaultAsync();

        if (globalUserSettings != null)
        {
            Console.WriteLine($"Loaded global UserPreset for GroupId={groupId}");
            return ServiceResult<ResultSettingsDto>.Success(globalUserSettings.ToDto());
        }

        // 3. Ищем Default для конкретной группы
        var defaultSettings = await _context.StudySettings
            .Where(s => s.UserId == userId && s.GroupId == groupId && s.PresetName == "Default")
            .FirstOrDefaultAsync();

        if (defaultSettings == null)
        {
            Console.WriteLine($"Creating new Default for GroupId={groupId}");
            defaultSettings = new StudySettings()
            {
                StudySettingsId = Guid.NewGuid(),
                UserId = userId,
                GroupId = groupId,
                StudyOrder = StudyOrder.Random,
                MinRating = 0,
                MaxRating = 5,
                PresetName = "Default",
                CompletionThreshold = 5,
                ShuffleOnRepeat = true,
            };

            _context.StudySettings.Add(defaultSettings);
            await _context.SaveChangesAsync();
        }

        return ServiceResult<ResultSettingsDto>.Success(defaultSettings.ToDto());
    }
    
    public async Task<ServiceResult<ResultSettingsDto>> SaveStudySettingsAsync(Guid userId, CreateSettingsDto dto)
    {
        // НОВОЕ: Если сохраняем глобальные настройки, удаляем все групповые UserPreset
        if (dto.GroupId == null)
        {
            var groupPresets = await _context.StudySettings
                .Where(s => s.UserId == userId && s.GroupId != null && s.PresetName == "UserPreset")
                .ToListAsync();
            
            if (groupPresets.Any())
            {
                _context.StudySettings.RemoveRange(groupPresets);
                Console.WriteLine($"Удалено {groupPresets.Count} групповых UserPreset при сохранении глобальных настроек");
            }
        }

        var existPreset = await _context.StudySettings
            .FirstOrDefaultAsync(s => s.UserId == userId && s.GroupId == dto.GroupId && s.PresetName == "UserPreset");

        if (existPreset != null)
        {
            Console.WriteLine($"Updating existing UserPreset: GroupId={dto.GroupId}");
        
            existPreset.MinRating = dto.MinRating;
            existPreset.MaxRating = dto.MaxRating;
            existPreset.StudyOrder = dto.StudyOrder;
            existPreset.CompletionThreshold = dto.CompletionThreshold;
            existPreset.ShuffleOnRepeat = dto.ShuffleOnRepeat;

            _context.StudySettings.Update(existPreset);
            await _context.SaveChangesAsync();
        
            Console.WriteLine($"Updated: MinRating={existPreset.MinRating}, MaxRating={existPreset.MaxRating}");
            return ServiceResult<ResultSettingsDto>.Success(existPreset.ToDto());
        }

        Console.WriteLine($"Creating new UserPreset: GroupId={dto.GroupId}");
    
        var newUserPreset = new StudySettings()
        {
            StudySettingsId = Guid.NewGuid(),
            UserId = userId,
            GroupId = dto.GroupId,
            PresetName = "UserPreset",
            MinRating = dto.MinRating,
            MaxRating = dto.MaxRating,
            StudyOrder = dto.StudyOrder,
            CompletionThreshold = dto.CompletionThreshold,
            ShuffleOnRepeat = dto.ShuffleOnRepeat
        };

        _context.StudySettings.Add(newUserPreset);
        await _context.SaveChangesAsync();

        return ServiceResult<ResultSettingsDto>.Success(newUserPreset.ToDto());
    }
    
    
    public async Task<ServiceResult<ResultSettingsDto>> GetDefaultSettingsAsync(Guid userId, Guid? groupId)
    {
        var defaultSettings = await _context.StudySettings
            .Where(s => s.UserId == userId && s.GroupId == groupId && s.PresetName == "Default")
            .FirstOrDefaultAsync();

        if (defaultSettings == null)
        {
            defaultSettings = new StudySettings()
            {
                StudySettingsId = Guid.NewGuid(),
                UserId = userId,
                GroupId = groupId,
                StudyOrder = StudyOrder.Random,
                MinRating = 0,
                MaxRating = 5,
                PresetName = "Default",
                CompletionThreshold = 5,
                ShuffleOnRepeat = true,
            };

            _context.StudySettings.Add(defaultSettings);
            await _context.SaveChangesAsync();
        }

        return ServiceResult<ResultSettingsDto>.Success(defaultSettings.ToDto());
    }
}