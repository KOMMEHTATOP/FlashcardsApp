using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.Models.DTOs.Cards.Requests;
using FlashcardsApp.Models.DTOs.Cards.Responses;

namespace FlashcardsApp.BLL.Interfaces;

public interface ICardBL
{
    Task<ServiceResult<IEnumerable<ResultCardDto>>> GetAllCardsAsync(Guid userId, int? targetRating);
    Task<ServiceResult<IEnumerable<ResultCardDto>>> GetCardsByGroupAsync(Guid groupId, Guid userId);
    Task<ServiceResult<ResultCardDto>> GetCardAsync(Guid cardId, Guid userId);
    Task<ServiceResult<ResultCardDto>> CreateCardAsync(Guid userId, Guid groupId, CreateCardDto dto);
    Task<ServiceResult<ResultCardDto>> UpdateCardAsync(Guid cardId, Guid userId, CreateCardDto dto);
    Task<ServiceResult<bool>> DeleteCardAsync(Guid cardId, Guid userId);
}