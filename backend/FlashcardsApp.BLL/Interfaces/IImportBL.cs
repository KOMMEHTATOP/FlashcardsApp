using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.Models.DTOs.Import.Requests;
using FlashcardsApp.Models.DTOs.Import.Responses;

namespace FlashcardsApp.BLL.Interfaces;

public interface IImportBL
{
    Task<ServiceResult<ImportResultDto>> ImportGroupWithCardsAsync(
        Guid userId, 
        ImportGroupWithCardsDto dto);
}
