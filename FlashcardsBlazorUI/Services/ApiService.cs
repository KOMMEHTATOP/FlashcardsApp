using FlashcardsBlazorUI.Models;
using System.Text.Json;

namespace FlashcardsBlazorUI.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private string? _currentToken;

        public ApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("FlashcardsAPI");
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
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
            }
            
            return loginResponse;
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

        // Существующие методы (теперь с токеном)
        public async Task<List<Group>> GetGroupsAsync()
        {
            var response = await _httpClient.GetAsync("http://localhost:5153/api/group");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Group>>(json, _jsonOptions) ?? new List<Group>();
        }
        
        public async Task<List<Card>> GetCardsAsync(string groupId)
        {
            var response = await _httpClient.GetAsync($"http://localhost:5153/api/groups/{groupId}/cards");
            response.EnsureSuccessStatusCode();
    
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Card>>(json, _jsonOptions) ?? new List<Card>();
        }
        
        public async Task<List<CardRating>> GetCardRatingsAsync(string cardId)
        {
            var response = await _httpClient.GetAsync($"http://localhost:5153/api/cards/{cardId}/ratings");
            response.EnsureSuccessStatusCode();
    
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CardRating>>(json, _jsonOptions) ?? new List<CardRating>();
        }

        public async Task<CardRating> RateCardAsync(string cardId, int rating)
        {
            var ratingData = new { rating };
            var json = JsonSerializer.Serialize(ratingData);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
    
            var response = await _httpClient.PostAsync($"http://localhost:5153/api/cards/{cardId}/ratings", content);
            response.EnsureSuccessStatusCode();
    
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CardRating>(responseJson, _jsonOptions);
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
    }
}