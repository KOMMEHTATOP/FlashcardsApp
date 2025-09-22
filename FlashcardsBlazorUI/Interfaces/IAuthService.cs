using FlashcardsAppContracts.DTOs.Responses;

namespace FlashcardsBlazorUI.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(string email, string password);
    Task LogoutAsync();
    Task<string?> GetTokenAsync();
}
