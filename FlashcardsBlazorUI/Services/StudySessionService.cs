using FlashcardsAppContracts.DTOs.Requests;
using FlashcardsAppContracts.DTOs.Responses;

namespace FlashcardsBlazorUI.Services;

public class StudySessionService : BaseApiService
{
    public StudySessionService(IHttpClientFactory httpClientFactory) 
        : base(httpClientFactory)
    {
    }

    public async Task<ResultStudySessionDto?> StartSessionAsync(Guid groupId, bool useDefaultSettings = false)
    {
        try
        {
            var url = $"api/StudySession/start?groupId={groupId}&useDefaultSettings={useDefaultSettings}";
            var response = await _httpClient.PostAsync(url, null);
    
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<ResultStudySessionDto>(json, _jsonOptions);
            }
    
            Console.WriteLine($"Ошибка запуска сессии: {response.StatusCode}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Исключение при запуске сессии: {ex.Message}");
            return null;
        }
    }
    
    public async Task<ResultSettingsDto?> GetSettingsAsync(Guid? groupId)
    {
        try
        {
            var url = groupId.HasValue 
                ? $"api/StudySettings?groupId={groupId}" 
                : "api/StudySettings";
            
            var response = await GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<ResultSettingsDto>(json, _jsonOptions);
            }
            
            Console.WriteLine($"Ошибка получения настроек: {response.StatusCode}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Исключение при получении настроек: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> SaveSettingsAsync(CreateSettingsDto settings)
    {
        try
        {
            var response = await PostAsJsonAsync("api/StudySettings", settings);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Исключение при сохранении настроек: {ex.Message}");
            return false;
        }
    }
}