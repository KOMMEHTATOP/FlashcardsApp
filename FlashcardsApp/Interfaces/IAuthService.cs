using FlashcardsApp.Models;
using FlashcardsApp.Services;
using FlashcardsAppContracts.DTOs.Auth.Requests;
using FlashcardsAppContracts.DTOs.Auth.Responses;
using FlashcardsAppContracts.DTOs.Requests;

namespace FlashcardsApp.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<RegisterUserDto>> Register(RegisterModel model);
    Task<ServiceResult<User>> Login(LoginModel model);
    Task<ServiceResult<RefreshTokenResult>> RefreshAccessToken(string refreshTokenValue, string ipAddress, string userAgent);
    Task<ServiceResult<bool>> Logout(string? refreshTokenValue);
}
