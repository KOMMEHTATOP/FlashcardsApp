using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.DTOs.Auth.Requests;
using FlashcardsApp.Models.DTOs.Auth.Responses;
using FlashcardsApp.Models.DTOs.Requests;

namespace FlashcardsApp.BLL.Interfaces;

public interface IAuthBL
{
    Task<ServiceResult<RegisterUserDto>> Register(RegisterModel model);
    Task<ServiceResult<User>> Login(LoginModel model);
    Task<ServiceResult<RefreshTokenResult>> RefreshAccessToken(string refreshTokenValue, string ipAddress, string userAgent);
    Task<ServiceResult<bool>> Logout(string? refreshTokenValue);
}
