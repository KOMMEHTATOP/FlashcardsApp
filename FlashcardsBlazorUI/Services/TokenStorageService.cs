using FlashcardsBlazorUI.Interfaces;

namespace FlashcardsBlazorUI.Services;


public class TokenStorageService : ITokenStorageService
{
    private string? _currentToken;

    public void SetToken(string token)
    {
        _currentToken = token;
    }

    public string? GetToken()
    {
        return _currentToken;
    }

    public void ClearToken()
    {
        _currentToken = null;
    }
}