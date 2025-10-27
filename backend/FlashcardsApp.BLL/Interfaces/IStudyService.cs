using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.Models.DTOs.Study.Requests;
using FlashcardsApp.Models.DTOs.Study.Responses;

namespace FlashcardsApp.BLL.Interfaces;

/// <summary>
/// Сервис для управления процессом изучения карточек и записи прогресса
/// </summary>
public interface IStudyService
{
    /// <summary>
    /// Записывает сессию изучения карточки и начисляет награды.
    /// Выполняет полный цикл: расчет XP, сохранение истории, начисление наград, обновление streak.
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="dto">Данные сессии изучения (CardId, Rating)</param>
    /// <returns>Результат с наградами: заработанный XP, level up, streak</returns>
    Task<ServiceResult<StudyRewardDto>> RecordStudySessionAsync(Guid userId, RecordStudyDto dto);
    
    /// <summary>
    /// Получает историю изучения карточек пользователя.
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="limit">Максимальное количество записей (по умолчанию 50)</param>
    /// <returns>Список записей истории изучения</returns>
    Task<ServiceResult<IEnumerable<StudyHistoryDto>>> GetStudyHistoryAsync(Guid userId, int? limit = 50);
}
