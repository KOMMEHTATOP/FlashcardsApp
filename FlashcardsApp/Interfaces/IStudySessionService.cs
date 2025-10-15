using FlashcardsApp.Services;
using FlashcardsAppContracts.DTOs.Responses;

namespace FlashcardsApp.Interfaces;

public interface IStudySessionService
{
    Task<ServiceResult<ResultStudySessionDto>> StartSessionAsync(Guid userId, Guid groupId);
}
