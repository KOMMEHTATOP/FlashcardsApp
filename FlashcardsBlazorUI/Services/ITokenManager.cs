using Microsoft.JSInterop;

namespace FlashcardsBlazorUI.Services
{
    public interface ITokenManager
    {
        string? Token { get; }
        bool IsAuthenticated { get; }
        Task InitializeAsync();
        Task SetTokenAsync(string token);
        Task ClearTokenAsync();
    }

    public class TokenManager : ITokenManager
    {
        private readonly IServiceProvider _serviceProvider;
        private string? _currentToken;

        public TokenManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string? Token => _currentToken;
        public bool IsAuthenticated => !string.IsNullOrEmpty(_currentToken);

        public async Task InitializeAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var jsRuntime = scope.ServiceProvider.GetRequiredService<IJSRuntime>();
        
            try
            {
                var token = await jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _currentToken = token;
                }
            }
            catch
            {
                // Игнорируем ошибки localStorage во время SSR/prerendering
            }
        }

        public async Task SetTokenAsync(string token)
        {
            _currentToken = token;
            using var scope = _serviceProvider.CreateScope();
            var jsRuntime = scope.ServiceProvider.GetRequiredService<IJSRuntime>();

            _currentToken = token;
            try
            {
                await jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
            }
            catch
            {
                // Игнорируем ошибки localStorage в SSR
            }
        }

        public async Task ClearTokenAsync()
        {
            _currentToken = null;
            using var scope = _serviceProvider.CreateScope();
            var jsRuntime = scope.ServiceProvider.GetRequiredService<IJSRuntime>();

            _currentToken = null;
            try
            {
                await jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
            }
            catch
            {
                // Игнорируем ошибки localStorage
            }
        }
    }
}
