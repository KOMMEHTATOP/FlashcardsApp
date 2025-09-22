using FlashcardsAppContracts.DTOs.Requests;
using FlashcardsAppContracts.DTOs.Responses;
using FlashcardsBlazorUI.Helpers;
using FlashcardsBlazorUI.Interfaces;

namespace FlashcardsBlazorUI.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly JwtAuthenticationStateProvider _authStateProvider;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        HttpClient httpClient,
        JwtAuthenticationStateProvider authStateProvider,
        ILogger<AuthService> logger)
    {
        _httpClient = httpClient;
        _authStateProvider = authStateProvider;
        _logger = logger;
    }

    public async Task<LoginResponse?> LoginAsync(string email, string password)
    {
        try
        {
            _logger.LogInformation("Попытка входа для пользователя: {Email}", email);

            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginRequest);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Неудачная попытка входа для пользователя {Email}. Status: {StatusCode}", 
                    email, response.StatusCode);
                return null;
            }

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            
            if (loginResponse == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _logger.LogWarning("Пустой ответ от API авторизации");
                return null;
            }

            var loginSuccess = await _authStateProvider.LoginAsync(loginResponse.Token);
            
            if (!loginSuccess)
            {
                _logger.LogError("Ошибка при обработке токена");
                return null;
            }

            _logger.LogInformation("Пользователь {Email} успешно вошел в систему", email);
            return loginResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при входе пользователя {Email}", email);
            return null;
        }
    }

    public async Task LogoutAsync()
    {
        await _authStateProvider.LogoutAsync();
        _logger.LogInformation("Пользователь вышел из системы");
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _authStateProvider.GetTokenAsync();
    }
}