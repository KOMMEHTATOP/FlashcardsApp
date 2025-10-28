using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.Models.DTOs.Study.Responses;

namespace FlashcardsApp.BLL.Interfaces;

public interface IStudySessionBL
{
    Task<ServiceResult<ResultStudySessionDto>> StartSessionAsync(Guid userId, Guid groupId);
}
