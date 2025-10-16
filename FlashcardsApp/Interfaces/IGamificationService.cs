using FlashcardsApp.Services;
using FlashcardsAppContracts.DTOs.Statistics.Responses;

namespace FlashcardsApp.Interfaces;

/// <summary>
/// Сервис для управления игровыми механиками: XP, уровни, streak
/// </summary>
public interface IGamificationService
{
    /// <summary>
    /// Рассчитывает XP за изученную карточку.
    /// Учитывает: качество ответа (оценка) и streak пользователя.
    /// Формула: XP = BaseXP × Quality × Streak
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="cardId">ID карточки</param>
    /// <param name="rating">Оценка от 1 до 5</param>
    /// <returns>Количество заработанного XP</returns>
    Task<int> CalculateXPForCardAsync(Guid userId, Guid cardId, int rating);

    /// <summary>
    /// Добавляет XP пользователю и проверяет повышение уровня.
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="xp">Количество XP для добавления</param>
    /// <returns>(leveledUp: был ли level up, newLevel: новый уровень)</returns>
    Task<ServiceResult<(bool leveledUp, int newLevel)>> AddXPToUserAsync(Guid userId, int xp);

    /// <summary>
    /// Обновляет streak пользователя на основе даты последнего занятия.
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <returns>True если streak увеличился, False если остался прежним или сбросился</returns>
    Task<ServiceResult<bool>> UpdateStreakAsync(Guid userId);

    /// <summary>
    /// Рассчитывает сколько XP требуется для достижения указанного уровня.
    /// </summary>
    /// <param name="level">Целевой уровень</param>
    /// <returns>Общее количество XP для достижения уровня</returns>
    int CalculateXPForLevel(int level);

    /// <summary>
    /// Генерирует мотивационное сообщение для пользователя.
    /// </summary>
    Task<ServiceResult<MotivationalMessageDto>> GetMotivationalMessageAsync(Guid userId);
}
