using FlashcardsBlazorUI.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FlashcardsBlazorUI.Helpers;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly ProtectedSessionStorage _sessionStorage;
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<JwtAuthenticationStateProvider> _logger;
    private readonly ITokenStorageService _tokenStorage;
    
    private ClaimsPrincipal _cachedUser = new(new ClaimsIdentity());
    private Timer? _tokenExpiryTimer;
    private string? _currentToken;

    private const string TOKEN_KEY = "jwt_token";
    
    private static int _instanceCounter = 0;
    private readonly int _instanceId;

    public JwtAuthenticationStateProvider(
        ProtectedSessionStorage sessionStorage,
        IJSRuntime jsRuntime,
        ILogger<JwtAuthenticationStateProvider> logger,
        ITokenStorageService tokenStorage)  
    {
        _sessionStorage = sessionStorage;
        _jsRuntime = jsRuntime;
        _logger = logger;
        _tokenStorage = tokenStorage;

        _instanceId = Interlocked.Increment(ref _instanceCounter);
        _logger.LogInformation("JwtAuthenticationStateProvider создан, instance {InstanceId}", _instanceId);
    }

    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        _logger.LogInformation("GetAuthenticationStateAsync вызван в instance {InstanceId}, _cachedUser.IsAuthenticated = {IsAuth}", 
            _instanceId, _cachedUser.Identity?.IsAuthenticated);
        
        // Проверяем, можем ли мы использовать JavaScript interop
        // Во время prerendering JavaScript недоступен
        try
        {
            await _jsRuntime.InvokeVoidAsync("eval", "null");
        }
        catch (InvalidOperationException)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        try
        {
            // Пытаемся загрузить токен из ProtectedSessionStorage
            var storedToken = await _sessionStorage.GetAsync<string>(TOKEN_KEY);
            
            if (storedToken.Success && !string.IsNullOrEmpty(storedToken.Value))
            {
                var user = await ValidateAndCreateUser(storedToken.Value);
                if (user.Identity?.IsAuthenticated == true)
                {
                    _cachedUser = user;
                    _currentToken = storedToken.Value;
                    _tokenStorage.SetToken(storedToken.Value); 
                    StartTokenExpiryTimer(storedToken.Value);
                    return new AuthenticationState(user);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ошибка при восстановлении JWT токена из session storage");
            await ClearStoredToken();
        }

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public async Task<bool> LoginAsync(string token)
    {
        var user = await ValidateAndCreateUser(token);
    
        if (user.Identity?.IsAuthenticated != true)
            return false;

        try
        {
            await _sessionStorage.SetAsync(TOKEN_KEY, token);
            _tokenStorage.SetToken(token);
        
            // Сохраняем в instance переменные
            _cachedUser = user;
            _currentToken = token;
            
        
            StartTokenExpiryTimer(token);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        
            _logger.LogInformation("Instance {InstanceId}: Пользователь успешно аутентифицирован", _instanceId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при сохранении JWT токена");
            return false;
        }
    }

    
    public async Task LogoutAsync()
    {
        _logger.LogInformation("Instance {InstanceId}: LogoutAsync вызван", _instanceId);

        try
        {
            await ClearStoredToken();
            _tokenStorage.ClearToken();
            StopTokenExpiryTimer();
        
            _cachedUser = new ClaimsPrincipal(new ClaimsIdentity());
            _currentToken = null;

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_cachedUser)));
        
            _logger.LogInformation("Пользователь успешно вышел из системы");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при выходе из системы");
        }
    }
    
    public async Task<string?> GetTokenAsync()
    {
        if (!string.IsNullOrEmpty(_currentToken))
        {
            return _currentToken;
        }

        // Проверяем доступность JavaScript
        try
        {
            await _jsRuntime.InvokeVoidAsync("eval", "null");
        }
        catch (InvalidOperationException)
        {
            // JavaScript недоступен (prerendering)
            return null;
        }

        try
        {
            var storedToken = await _sessionStorage.GetAsync<string>(TOKEN_KEY);
            if (storedToken.Success && !string.IsNullOrEmpty(storedToken.Value))
            {
                _currentToken = storedToken.Value;
                _logger.LogInformation("Токен получен из storage: {TokenStart}...", _currentToken.Substring(0, 20));
                return _currentToken;
            }
            else
            {
                _logger.LogWarning("Токен не найден в storage");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ошибка при получении токена");
        }

        return null;
    }

    private async Task<ClaimsPrincipal> ValidateAndCreateUser(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            
            if (!handler.CanReadToken(token))
            {
                _logger.LogWarning("Невалидный JWT токен");
                return new ClaimsPrincipal(new ClaimsIdentity());
            }

            var jwtToken = handler.ReadJwtToken(token);
            
            // Проверяем срок действия токена
            if (jwtToken.ValidTo <= DateTime.UtcNow)
            {
                _logger.LogWarning("JWT токен истек");
                await ClearStoredToken();
                return new ClaimsPrincipal(new ClaimsIdentity());
            }

            // Создаем ClaimsPrincipal из JWT claims + добавляем сам токен
            var claims = jwtToken.Claims.ToList();
            // ВАЖНО: Добавляем токен в claims для AuthenticationHandler
            claims.Add(new Claim("jwt_token", token));
            
            var identity = new ClaimsIdentity(claims, "jwt");
            
            return new ClaimsPrincipal(identity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при валидации JWT токена");
            return new ClaimsPrincipal(new ClaimsIdentity());
        }
    }

    private void StartTokenExpiryTimer(string token)
    {
        StopTokenExpiryTimer();
        
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            
            var expiryTime = jwtToken.ValidTo;
            var currentTime = DateTime.UtcNow;
            
            if (expiryTime <= currentTime)
            {
                _ = Task.Run(async () => await LogoutAsync());
                return;
            }

            var timeToExpiry = expiryTime - currentTime;
            
            // Выходим за 30 секунд до истечения токена
            var logoutTime = timeToExpiry.Subtract(TimeSpan.FromSeconds(30));
            
            if (logoutTime > TimeSpan.Zero)
            {
                _tokenExpiryTimer = new Timer(async _ => await LogoutAsync(), 
                    null, logoutTime, Timeout.InfiniteTimeSpan);
                
                _logger.LogInformation("Таймер выхода установлен на {Minutes:F1} минут", logoutTime.TotalMinutes);
            }
            else
            {
                _ = Task.Run(async () => await LogoutAsync());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при установке таймера истечения токена");
        }
    }

    private void StopTokenExpiryTimer()
    {
        _tokenExpiryTimer?.Dispose();
        _tokenExpiryTimer = null;
    }

    private async Task ClearStoredToken()
    {
        try
        {
            await _sessionStorage.DeleteAsync(TOKEN_KEY);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ошибка при удалении токена из storage");
        }
    }

    public void Dispose()
    {
        StopTokenExpiryTimer();
    }
}