using FlashcardsApp.DAL.Models;

namespace FlashcardsApp.Services.Interfaces;

/// <summary>
/// Сервис для генерации JWT и Refresh токенов
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Генерирует JWT Access Token
    /// </summary>
    string GenerateAccessToken(Guid userId, string email, IEnumerable<string>? roles = null);
    
    /// <summary>
    /// Генерирует JWT Access Token из User entity (convenience метод)
    /// </summary>
    Task<string> GenerateAccessToken(User user);
    
    /// <summary>
    /// Генерирует случайную строку для Refresh Token
    /// </summary>
    string GenerateRefreshTokenString();
    
    /// <summary>
    /// Генерирует и сохраняет Refresh Token в БД (для совместимости с контроллером)
    /// </summary>
    Task<RefreshToken> GenerateRefreshToken(Guid userId, string? ipAddress, string? userAgent);
    
    /// <summary>
    /// Валидирует JWT токен и извлекает userId
    /// </summary>
    Guid? ValidateAccessToken(string token);
}
