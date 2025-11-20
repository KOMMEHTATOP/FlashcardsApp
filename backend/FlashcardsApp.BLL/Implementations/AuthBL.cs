using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.DAL;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.DTOs.Auth.Requests;
using FlashcardsApp.Models.DTOs.Auth.Responses;
using FlashcardsApp.Models.DTOs.Requests;
using FlashcardsApp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FlashcardsApp.BLL.Implementations;

public class AuthBL : IAuthBL
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuthBL> _logger;

    public AuthBL(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ITokenService tokenService,
        ApplicationDbContext context,
        ILogger<AuthBL> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResult<RegisterUserDto>> Register(RegisterModel model)
    {
        try
        {
            var existingLogin = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == model.Login);

            if (existingLogin != null)
            {
                return ServiceResult<RegisterUserDto>.Failure("Логин уже занят");
            }

            var existingEmail = await _userManager.FindByEmailAsync(model.Email);

            if (existingEmail != null)
            {
                return ServiceResult<RegisterUserDto>.Failure("Email уже зарегистрирован");
            }

            var user = new User
            {
                Login = model.Login, Role = model.Role ?? "User", UserName = model.Email, Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e =>
                {
                    return e.Description switch
                    {
                        { } desc when desc.Contains("Password") =>
                            "Пароль слишком простой. Используйте заглавные буквы, цифры и спецсимволы",
                        { } desc when desc.Contains("Email") =>
                            "Некорректный формат email",
                        { } desc when desc.Contains("UserName") =>
                            "Некорректное имя пользователя",
                        _ => e.Description
                    };
                }).ToArray();

                _logger.LogWarning("Ошибки регистрации: {Errors}", string.Join(", ", errors));
                return ServiceResult<RegisterUserDto>.Failure(errors);
            }

            _logger.LogInformation("Пользователь {Email} с логином {Login} успешно зарегистрирован",
                model.Email, model.Login);

            return ServiceResult<RegisterUserDto>.Success(
                new RegisterUserDto
            {
                IsSuccess = true,
                CreatedAt = DateTime.UtcNow,
                Message = "Пользователь успешно зарегистрирован."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при регистрации пользователя {Email}", model.Email);
            return ServiceResult<RegisterUserDto>.Failure("Произошла ошибка при регистрации");
        }
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

        user.LastLogin = model.LastLogin;
        _context.Users.Update(user);
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
            // Валидируем refresh token в БД
            var refreshToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt =>
                    rt.Token == refreshTokenValue &&
                    rt.ExpiresAt > DateTime.UtcNow &&
                    !rt.IsRevoked);

            if (refreshToken == null)
            {
                return ServiceResult<RefreshTokenResult>.Failure("Недействительный или истекший refresh токен");
            }

            // Получаем роли пользователя для нового access token
            var roles = await _userManager.GetRolesAsync(refreshToken.User);

            // Генерируем новый access token через TokenService
            var accessToken = _tokenService.GenerateAccessToken(
                refreshToken.User.Id,
                refreshToken.User.Email ?? string.Empty,
                roles);

            // Refresh Token Rotation: отзываем старый refresh token
            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;

            // Создаём новый refresh token
            var newRefreshTokenString = _tokenService.GenerateRefreshTokenString();
            var newRefreshToken = new RefreshToken
            {
                Token = newRefreshTokenString,
                UserId = refreshToken.UserId,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = ipAddress,
                CreatedByUserAgent = userAgent
            };

            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Tokens refreshed for user {UserId}", refreshToken.UserId);

            return ServiceResult<RefreshTokenResult>.Success(new RefreshTokenResult
            {
                AccessToken = accessToken, RefreshToken = newRefreshTokenString
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
                var refreshToken = await _context.RefreshTokens
                    .FirstOrDefaultAsync(rt => rt.Token == refreshTokenValue);

                if (refreshToken != null)
                {
                    refreshToken.IsRevoked = true;
                    refreshToken.RevokedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Refresh token revoked for user {UserId}", refreshToken.UserId);
                }
                else
                {
                    _logger.LogWarning("Refresh token not found during logout");
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

    /// <summary>
    /// Получить действующий refresh token из БД
    /// </summary>
    private async Task<RefreshToken?> GetValidRefreshTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt =>
                rt.Token == token &&
                rt.ExpiresAt > DateTime.UtcNow &&
                !rt.IsRevoked);
    }

    /// <summary>
    /// Очистка истекших токенов (можно вызывать периодически)
    /// </summary>
    public async Task CleanupExpiredTokensAsync()
    {
        var expiredTokens = await _context.RefreshTokens
            .Where(rt => rt.ExpiresAt < DateTime.UtcNow)
            .ToListAsync();

        if (expiredTokens.Any())
        {
            _context.RefreshTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Cleaned up {Count} expired refresh tokens", expiredTokens.Count);
        }
    }
}
