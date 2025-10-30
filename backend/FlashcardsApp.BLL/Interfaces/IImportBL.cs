using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.Models.DTOs.Import.Requests;
using FlashcardsApp.Models.DTOs.Import.Responses;

namespace FlashcardsApp.BLL.Interfaces;

public interface IImportBL
{
    /// <summary>
    /// Импортирует группу с карточками
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="dto">Данные для импорта</param>
    /// <returns>Результат импорта с детальной статистикой</returns>
    Task<ServiceResult<ImportResultDto>> ImportGroupWithCardsAsync(
        Guid userId, 
        ImportGroupWithCardsDto dto);
}
