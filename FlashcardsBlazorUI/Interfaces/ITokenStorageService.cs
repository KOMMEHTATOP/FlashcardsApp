namespace FlashcardsBlazorUI.Interfaces;

public interface ITokenStorageService
{
    void SetToken(string token);
    string? GetToken();
    void ClearToken();
}
