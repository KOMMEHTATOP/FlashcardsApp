using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.Models.DTOs.Requests;
using FlashcardsApp.Models.DTOs.Responses;

namespace FlashcardsApp.BLL.Interfaces;

public interface IStudySettingsService
{
    Task<ServiceResult<ResultSettingsDto>> GetStudySettingsAsync(Guid userId);
    Task<ServiceResult<ResultSettingsDto>> SaveStudySettingsAsync(Guid userId, CreateSettingsDto dto);
    Task<ServiceResult<ResultSettingsDto>> ResetToDefaultAsync(Guid userId);
}
