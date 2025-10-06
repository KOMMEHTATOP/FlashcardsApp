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
    
    public async Task<RegisterUserDto> RegisterUserAsync(RegisterModel model)
    {
        try
        {
            _logger.LogInformation("Попытка регистрации пользователя: {Email}", model.Email);

            // 1. Отправляем POST запрос на бэкенд
            // PostAsJsonAsync автоматически превращает model в JSON и отправляет
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", model);
        
            // 2. Читаем ответ от сервера и превращаем JSON обратно в объект
            // Это работает и для успеха (200 OK) и для ошибки (400 BadRequest)
            var registerResponse = await response.Content.ReadFromJsonAsync<RegisterUserDto>();
        
            // 3. Если по какой-то причине не смогли прочитать ответ
            if (registerResponse == null)
            {
                _logger.LogError("Пустой ответ от API регистрации");
                return new RegisterUserDto
                {
                    IsSuccess = false,
                    Message = "Ошибка связи с сервером"
                };
            }

            // 4. Логируем результат
            if (registerResponse.IsSuccess)
            {
                _logger.LogInformation("Пользователь {Email} успешно зарегистрирован", model.Email);
            }
            else
            {
                _logger.LogWarning("Неудачная регистрация для {Email}: {Message}", 
                    model.Email, registerResponse.Message);
            }

            // 5. Возвращаем результат - он уже содержит всю информацию
            // (IsSuccess, Message, Errors)
            return registerResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при регистрации пользователя {Email}", model.Email);
            return new RegisterUserDto
            {
                IsSuccess = false,
                Message = "Произошла ошибка при регистрации"
            };
        }
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