using FlashcardsBlazorUI.Models;
using Microsoft.JSInterop;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace FlashcardsBlazorUI.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IJSRuntime _jsRuntime;
        private string? _currentToken;

        public ApiService(IHttpClientFactory httpClientFactory, IJSRuntime jsRuntime)
        {
            _httpClient = httpClientFactory.CreateClient("FlashcardsAPI");
            _jsRuntime = jsRuntime;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        // Получение карточки
        public async Task<Card?> GetCardAsync(Guid cardId)
        {
            var response = await _httpClient.GetAsync($"api/cards/{cardId}");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await LogoutAsync();
                throw new UnauthorizedAccessException("Требуется авторизация");
            }
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<Card>();
        }

        // Инициализация: читаем токен из localStorage (вызов через JSRuntime)
        public async Task InitializeAsync()
        {
            try
            {
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
                if (!string.IsNullOrEmpty(token))
                {
                    SetAuthToken(token);
                }
            }
            catch
            {
                // Игнорируем ошибки localStorage во время SSR/prerendering
            }
        }

        // --- Аутентификация ---
        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginRequest);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("Неверные учетные данные");

            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseJson, _jsonOptions);

            if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
            {
                SetAuthToken(loginResponse.Token);
                try
                {
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", loginResponse.Token);
                }
                catch
                {
                    // Игнорируем ошибки localStorage в SSR
                }
            }

            return loginResponse!;
        }

        public async Task LogoutAsync()
        {
            ClearAuthToken();
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
            }
            catch
            {
                // Игнорируем ошибки localStorage
            }
        }

        public void SetAuthToken(string token)
        {
            _currentToken = token;
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public void ClearAuthToken()
        {
            _currentToken = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public bool IsAuthenticated => !string.IsNullOrEmpty(_currentToken);

        // --- Группы ---
        public async Task<List<Group>> GetGroupsAsync()
        {
            var response = await _httpClient.GetAsync("api/group");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await LogoutAsync();
                throw new UnauthorizedAccessException("Требуется авторизация");
            }
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Group>>(json, _jsonOptions) ?? new List<Group>();
        }

        public async Task<Group?> GetGroupAsync(Guid groupId)
        {
            var response = await _httpClient.GetAsync($"api/group/{groupId}");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await LogoutAsync();
                throw new UnauthorizedAccessException("Требуется авторизация");
            }
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Group>(json, _jsonOptions);
        }

        public async Task<Group> CreateGroupAsync(object groupData)
        {
            // Подготовим DTO корректно
            var createGroupDto = new {
                Name = ((dynamic)groupData).Name,
                Color = ((dynamic)groupData).Color
            };

            var response = await _httpClient.PostAsJsonAsync("api/group", createGroupDto);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await LogoutAsync();
                throw new UnauthorizedAccessException("Требуется авторизация");
            }
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Group>(responseJson, _jsonOptions)!;
        }

        public async Task<bool> UpdateGroupOrderAsync(List<ReorderGroupDto> groupOrders)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("api/group/reorder", groupOrders);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await LogoutAsync();
                    return false;
                }
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteGroupAsync(Guid groupId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/group/{groupId}");
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await LogoutAsync();
                    return false;
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка удаления группы: {ex.Message}");
                return false;
            }
        }

        // --- Карточки и рейтинги ---
        public async Task<List<Card>> GetCardsAsync(Guid groupId)
        {
            var response = await _httpClient.GetAsync($"api/groups/{groupId}/cards");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await LogoutAsync();
                throw new UnauthorizedAccessException("Требуется авторизация");
            }
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Card>>() ?? new();
        }

        public async Task<List<CardRating>> GetCardRatingsAsync(Guid cardId)
        {
            var response = await _httpClient.GetAsync($"api/cards/{cardId}/ratings");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await LogoutAsync();
                throw new UnauthorizedAccessException("Требуется авторизация");
            }
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CardRating>>(json, _jsonOptions) ?? new List<CardRating>();
        }

        public async Task<CardRating> RateCardAsync(Guid cardId, int rating)
        {
            var ratingData = new { rating };
            var response = await _httpClient.PostAsJsonAsync($"api/cards/{cardId}/ratings", ratingData);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await LogoutAsync();
                throw new UnauthorizedAccessException("Требуется авторизация");
            }
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CardRating>(responseJson, _jsonOptions)!;
        }

        public async Task<Card> CreateCardAsync(CreateCardDto cardDto, Guid groupId)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/groups/{groupId}/cards", cardDto);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await LogoutAsync();
                throw new UnauthorizedAccessException("Требуется авторизация");
            }
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Card>(responseJson, _jsonOptions)!;
        }
    }
}
