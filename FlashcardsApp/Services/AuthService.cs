using FlashcardsApp.Interfaces;
using FlashcardsApp.Models;
using FlashcardsAppContracts.DTOs.Auth.Requests;
using FlashcardsAppContracts.DTOs.Auth.Responses;
using FlashcardsAppContracts.DTOs.Requests;
using Microsoft.AspNetCore.Identity;

namespace FlashcardsApp.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<ServiceResult<RegisterUserDto>> Register(RegisterModel model)
    {
        var user = new User
        {
            Login = model.Login, UserName = model.Email, Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToArray();
            _logger.LogWarning($"Returning errors: {string.Join(", ", errors)}");

            return ServiceResult<RegisterUserDto>.Failure(errors);
        }

        _logger.LogInformation("User {Email} successfully registered", model.Email);

        return ServiceResult<RegisterUserDto>.Success(new RegisterUserDto
        {
            IsSuccess = true, Message = "Пользователь успешно зарегистрирован."
        });
    }

    public async Task<ServiceResult<User>> Login(LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            return ServiceResult<User>.Failure("Неверный логин или пароль");
        }

        var passwordValid = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

        if (!passwordValid.Succeeded)
        {
            return ServiceResult<User>.Failure("Неверный логин или пароль");
        }

        _logger.LogInformation("User {UserId} successfully logged in", user.Id);

        return ServiceResult<User>.Success(user);
    }

    /// <summary>
    /// Обновить access token используя refresh token
    /// </summary>
    public async Task<ServiceResult<RefreshTokenResult>> RefreshAccessToken(
        string refreshTokenValue,
        string ipAddress,
        string userAgent)
    {
        try
        {
            // Валидируем старый refresh token
            var refreshToken = await _tokenService.ValidateRefreshToken(refreshTokenValue);

            if (refreshToken == null)
            {
                return ServiceResult<RefreshTokenResult>.Failure("Недействительный или истекший refresh токен");
            }

            // Генерируем новый access token
            var accessToken = _tokenService.GenerateAccessToken(refreshToken.User);

            // Refresh Token Rotation: отзываем старый, создаём новый
            await _tokenService.RevokeRefreshToken(refreshTokenValue);
            var newRefreshToken = await _tokenService.GenerateRefreshToken(
                refreshToken.UserId,
                ipAddress,
                userAgent);

            _logger.LogInformation("Tokens refreshed for user {UserId}", refreshToken.UserId);

            return ServiceResult<RefreshTokenResult>.Success(new RefreshTokenResult
            {
                AccessToken = accessToken, RefreshToken = newRefreshToken.Token
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing tokens");
            return ServiceResult<RefreshTokenResult>.Failure("Не удалось обновить токены");
        }
    }

    /// <summary>
    /// Выйти из системы (отозвать refresh token)
    /// </summary>
    public async Task<ServiceResult<bool>> Logout(string? refreshTokenValue)
    {
        try
        {
            if (!string.IsNullOrEmpty(refreshTokenValue))
            {
                var revoked = await _tokenService.RevokeRefreshToken(refreshTokenValue);

                if (!revoked)
                {
                    _logger.LogWarning("Failed to revoke refresh token during logout");
                }
            }

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return ServiceResult<bool>.Failure("Ошибка при выходе из системы");
        }
    }
}
