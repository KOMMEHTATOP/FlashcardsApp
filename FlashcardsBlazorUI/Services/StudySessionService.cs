using FlashcardsAppContracts.DTOs.Responses;

namespace FlashcardsBlazorUI.Services;

public class StudySessionService
{
    private readonly HttpClient _httpClient;

    public StudySessionService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("FlashcardsAPI");
    }

    public async Task<ResultStudySessionDto?> StartSessionAsync(Guid groupId)
    {
        try
        {
            var response = await _httpClient.PostAsync($"api/StudySession/start?groupId={groupId}", null);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ResultStudySessionDto>();
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
}
