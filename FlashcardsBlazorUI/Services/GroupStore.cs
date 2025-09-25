using FlashcardsAppContracts.DTOs.Responses;
using FlashcardsBlazorUI.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlashcardsBlazorUI.Services;

public class GroupStore : IDisposable
{
    private readonly HttpClient _http;
    private readonly IGroupNotificationService _notificationService;
    private bool _disposed;

    public List<ResultGroupDto> Groups { get; private set; } = new();
    public event Action? GroupsChanged;
    
    public GroupStore(HttpClient http, IGroupNotificationService notificationService)
    {
        _http = http;
        _notificationService = notificationService;
        _notificationService.OnGroupsReordered += HandleGroupsReordered;
    }

    public async Task RefreshAsync()
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new JsonStringEnumConverter());

            var data = await _http.GetFromJsonAsync<List<ResultGroupDto>>("api/group", options);
            
            if (data is not null)
            {
                Groups = data;
                GroupsChanged?.Invoke();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GroupStore: Ошибка при загрузке групп: {ex.Message}");
            throw;
        }
    }

    private void HandleGroupsReordered(string sourceContainer)
    {
        Task.Run(async () =>
        {
            try
            {
                await RefreshAsync();
                
                // Небольшая задержка, чтобы компоненты успели обработать обновление
                await Task.Delay(100);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GroupStore: Ошибка при обновлении после уведомления: {ex.Message}");
            }
        });
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _notificationService.OnGroupsReordered -= HandleGroupsReordered;
            _disposed = true;
        }
    }
}