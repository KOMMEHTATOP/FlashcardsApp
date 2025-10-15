using FlashcardsApp.Services;
using FlashcardsAppContracts.DTOs.Requests;
using FlashcardsAppContracts.DTOs.Responses;


namespace FlashcardsApp.Interfaces;

public interface ICardRatingService
{
    Task<ServiceResult<IEnumerable<ResultCardRatingDto>>> GetCardRatingsAsync(Guid userId, Guid cardId);
    Task<ServiceResult<ResultCardRatingDto>> CreateCardRatingAsync(Guid cardId, Guid userId, CreateCardRatingDto dto);
    Task<ServiceResult<bool>> DeleteCardRatingsAsync(Guid cardId, Guid userId);
}
