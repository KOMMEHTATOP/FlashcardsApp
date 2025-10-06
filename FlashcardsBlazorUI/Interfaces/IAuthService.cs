using FlashcardsAppContracts.DTOs.Requests;
using FlashcardsAppContracts.DTOs.Responses;

namespace FlashcardsBlazorUI.Interfaces;

public interface IAuthService
{
    Task<RegisterUserDto> RegisterUserAsync(RegisterModel model);
    Task<LoginResponse?> LoginAsync(string email, string password);
    Task LogoutAsync();
    Task<string?> GetTokenAsync();
}
