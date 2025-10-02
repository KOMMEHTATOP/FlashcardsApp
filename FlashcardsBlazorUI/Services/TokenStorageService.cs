using FlashcardsBlazorUI.Interfaces;

namespace FlashcardsBlazorUI.Services;


public class TokenStorageService : ITokenStorageService
{
    private string? _currentToken;

    public void SetToken(string token)
    {
        _currentToken = token;
        Console.WriteLine($"[TokenStorageService] Токен сохранен: {token.Substring(0, Math.Min(20, token.Length))}...");
    }

    public string? GetToken()
    {
        Console.WriteLine($"[TokenStorageService] Запрос токена. Значение: {(_currentToken != null ? _currentToken.Substring(0, Math.Min(20, _currentToken.Length)) + "..." : "NULL")}");
        return _currentToken;
    }

    public void ClearToken()
    {
        _currentToken = null;
    }
}