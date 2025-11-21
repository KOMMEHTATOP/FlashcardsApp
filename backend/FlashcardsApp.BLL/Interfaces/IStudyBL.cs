using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.Models.DTOs.Study.Requests;
using FlashcardsApp.Models.DTOs.Study.Responses;

namespace FlashcardsApp.BLL.Interfaces;

public interface IStudyBL
{
    Task<ServiceResult<StudyRewardDto>> RecordStudySessionAsync(Guid userId, RecordStudyDto dto);
    Task<ServiceResult<IEnumerable<StudyHistoryDto>>> GetStudyHistoryAsync(Guid userId, int? limit = 50);
}
