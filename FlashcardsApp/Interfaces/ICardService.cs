using FlashcardsApp.Services;
using FlashcardsAppContracts.DTOs.Requests;
using FlashcardsAppContracts.DTOs.Responses;

namespace FlashcardsApp.Interfaces;

/// <summary>
/// Сервис для управления карточками (flashcards)
/// </summary>
public interface ICardService
{
    /// <summary>
    /// Получает все карточки пользователя с опциональной фильтрацией по рейтингу
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="targetRating">Максимальный рейтинг для фильтрации (опционально)</param>
    /// <returns>Список карточек пользователя</returns>
    Task<ServiceResult<IEnumerable<ResultCardDto>>> GetAllCardsAsync(Guid userId, int? targetRating);

    /// <summary>
    /// Получает все карточки из конкретной группы
    /// </summary>
    /// <param name="groupId">ID группы</param>
    /// <param name="userId">ID пользователя для проверки доступа</param>
    /// <returns>Список карточек из группы</returns>
    Task<ServiceResult<IEnumerable<ResultCardDto>>> GetCardsByGroupAsync(Guid groupId, Guid userId);

    /// <summary>
    /// Получает одну карточку по ID
    /// </summary>
    /// <param name="cardId">ID карточки</param>
    /// <param name="userId">ID пользователя для проверки доступа</param>
    /// <returns>Карточка или ошибка, если не найдена</returns>
    Task<ServiceResult<ResultCardDto>> GetCardAsync(Guid cardId, Guid userId);

    /// <summary>
    /// Создает новую карточку
    /// </summary>
    /// <param name="userId">ID пользователя-владельца</param>
    /// <param name="groupId">ID группы, в которую добавляется карточка</param>
    /// <param name="dto">Данные для создания карточки</param>
    /// <returns>Созданная карточка или ошибка</returns>
    Task<ServiceResult<ResultCardDto>> CreateCardAsync(Guid userId, Guid groupId, CreateCardDto dto);

    /// <summary>
    /// Обновляет существующую карточку
    /// </summary>
    /// <param name="cardId">ID карточки для обновления</param>
    /// <param name="userId">ID пользователя для проверки доступа</param>
    /// <param name="dto">Новые данные карточки</param>
    /// <returns>Обновленная карточка или ошибка</returns>
    Task<ServiceResult<ResultCardDto>> UpdateCardAsync(Guid cardId, Guid userId, CreateCardDto dto);

    /// <summary>
    /// Удаляет карточку
    /// </summary>
    /// <param name="cardId">ID карточки для удаления</param>
    /// <param name="userId">ID пользователя для проверки доступа</param>
    /// <returns>true если удалена успешно, иначе ошибка</returns>
    Task<ServiceResult<bool>> DeleteCardAsync(Guid cardId, Guid userId);
}