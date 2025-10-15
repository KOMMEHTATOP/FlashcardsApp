using FlashcardsApp.Services;
using FlashcardsAppContracts.DTOs.Requests;
using FlashcardsAppContracts.DTOs.Responses;

namespace FlashcardsApp.Interfaces;

public interface IStudySettingsService
{
    Task<ServiceResult<ResultSettingsDto>> GetStudySettingsAsync(Guid userId);
    Task<ServiceResult<ResultSettingsDto>> SaveStudySettingsAsync(Guid userId, CreateSettingsDto dto);
    Task<ServiceResult<ResultSettingsDto>> ResetToDefaultAsync(Guid userId);
}
