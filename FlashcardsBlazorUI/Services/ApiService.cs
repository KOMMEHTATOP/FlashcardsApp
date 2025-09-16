using FlashcardsBlazorUI.Models;
using Microsoft.JSInterop;
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
        public async Task<Card?> GetCardAsync(Guid cardId)
        {
            var resp = await _httpClient.GetAsync($"api/cards/{cardId}");
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<Card>();
        }

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
                // Ignore localStorage errors in SSR mode
            }
        }
        

        // Методы аутентификации
        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("http://localhost:5153/api/auth/login", content);
            response.EnsureSuccessStatusCode();
            
            var responseJson = await response.Content.ReadAsStringAsync();
            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseJson, _jsonOptions);
            
            // Сохраняем токен для последующих запросов
            if (loginResponse != null)
            {
                SetAuthToken(loginResponse.Token);
                try
                {
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", loginResponse.Token);
                }
                catch
                {
                    // Ignore localStorage errors in SSR mode
                }
            }
            
            return loginResponse;
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
                // Ignore localStorage errors
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

        public async Task<List<Group>> GetGroupsAsync()
        {
            return await ExecuteRequestAsync(async () =>
            {
                var response = await _httpClient.GetAsync("api/group");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Group>>(json, _jsonOptions) ?? new List<Group>();
            });
        }
        
        public async Task<List<Card>> GetCardsAsync(Guid groupId)
        {
            var response = await _httpClient.GetAsync($"api/groups/{groupId}/cards");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Card>>() ?? new();
        }
        
        public async Task<List<CardRating>> GetCardRatingsAsync(Guid cardId)
        {
            var response = await _httpClient.GetAsync($"api/cards/{cardId}/ratings");
            response.EnsureSuccessStatusCode();
    
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CardRating>>(json, _jsonOptions) ?? new List<CardRating>();
        }

        public async Task<CardRating> RateCardAsync(Guid cardId, int rating)
        {
            var ratingData = new { rating };
            var json = JsonSerializer.Serialize(ratingData);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
    
            var response = await _httpClient.PostAsync($"api/cards/{cardId}/ratings", content);
            response.EnsureSuccessStatusCode();
    
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CardRating>(responseJson, _jsonOptions);
        }
        
        public async Task<Card> CreateCardAsync(CreateCardDto cardDto, Guid groupId)
        {
            var json = JsonSerializer.Serialize(cardDto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"api/groups/{groupId}/cards", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Card>(responseJson, _jsonOptions);
        }
        
        public async Task<Group?> GetGroupAsync(Guid groupId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/group/{groupId}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Group>(json, _jsonOptions);
            }
            catch
            {
                return null;
            }
        }
        
        public async Task<Group> CreateGroupAsync(object groupData)
        {
            // Преобразуем объект в правильный формат для API
            var createGroupDto = new {
                Name = ((dynamic)groupData).Name,
                Color = int.Parse(((dynamic)groupData).Color) // Преобразуем строку в число enum
            };
            
            var json = JsonSerializer.Serialize(createGroupDto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
    
            var response = await _httpClient.PostAsync("http://localhost:5153/api/group", content);
            response.EnsureSuccessStatusCode();
    
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Group>(responseJson, _jsonOptions);
        }
        
        public async Task<bool> UpdateGroupOrderAsync(List<ReorderGroupDto> groupOrders)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("http://localhost:5153/api/group/reorder", groupOrders);
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
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка удаления группы: {ex.Message}");
                return false;
            }
        }
        
        private async Task<T> ExecuteRequestAsync<T>(Func<Task<T>> request)
        {
            try
            {
                return await request();
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("401"))
            {
                await LogoutAsync();
                throw new UnauthorizedAccessException("Требуется авторизация");
            }
        }

    }
}