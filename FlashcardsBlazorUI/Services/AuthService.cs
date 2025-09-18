using FlashcardsAppContracts.DTOs.Requests;
using FlashcardsAppContracts.DTOs.Responses;
using System.Net;
using System.Text.Json;

namespace FlashcardsBlazorUI.Services
{
    public class AuthService : BaseApiService
    {
        public AuthService(IHttpClientFactory httpClientFactory, ITokenManager tokenManager) 
            : base(httpClientFactory, tokenManager)
        {
        }

        // Проксирует свойства TokenManager для совместимости
        public bool IsAuthenticated => _tokenManager.IsAuthenticated;
        
        public async Task InitializeAsync()
        {
            await _tokenManager.InitializeAsync();
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            Console.WriteLine($"Отправляю логин запрос: {JsonSerializer.Serialize(loginRequest)}");
            
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginRequest);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("Неверные учетные данные");

            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Ответ от сервера: {responseJson}");
            
            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseJson, _jsonOptions);
            Console.WriteLine($"Десериализованный токен: {loginResponse?.Token}");

            if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
            {
                Console.WriteLine("Сохраняю токен через TokenManager...");
                await _tokenManager.SetTokenAsync(loginResponse.Token);
                Console.WriteLine("Токен сохранен через TokenManager");
            }
            else
            {
                Console.WriteLine("Токен пустой или null!");
            }

            return loginResponse!;
        }

        public async Task LogoutAsync()
        {
            await _tokenManager.ClearTokenAsync();
        }
    }
}