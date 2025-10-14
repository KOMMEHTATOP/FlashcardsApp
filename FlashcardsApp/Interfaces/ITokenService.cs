using FlashcardsApp.Models;

namespace FlashcardsApp.Interfaces;

/// <summary>
/// Сервис для работы с JWT Access токенами и Refresh токенами
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Генерирует JWT Access Token для пользователя
    /// </summary>
    /// <param name="user">Пользователь, для которого генерируется токен</param>
    /// <returns>JWT токен в виде строки</returns>
    string GenerateAccessToken(User user);

    /// <summary>
    /// Генерирует Refresh Token и сохраняет его в базе данных
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="ipAddress">IP адрес клиента (опционально)</param>
    /// <param name="userAgent">User Agent браузера (опционально)</param>
    /// <returns>Созданный Refresh Token</returns>
    Task<RefreshToken> GenerateRefreshToken(Guid userId, string? ipAddress, string? userAgent);

    /// <summary>
    /// Проверяет валидность Refresh Token
    /// </summary>
    /// <param name="token">Строка токена</param>
    /// <returns>RefreshToken если валиден, null если невалиден или истек</returns>
    Task<RefreshToken?> ValidateRefreshToken(string token);

    /// <summary>
    /// Отзывает (деактивирует) Refresh Token
    /// </summary>
    /// <param name="token">Строка токена для отзыва</param>
    /// <returns>true если токен успешно отозван, false если токен не найден</returns>
    Task<bool> RevokeRefreshToken(string token);

    /// <summary>
    /// Удаляет истекшие токены из базы данных
    /// </summary>
    Task CleanupExpiredTokens();
}
