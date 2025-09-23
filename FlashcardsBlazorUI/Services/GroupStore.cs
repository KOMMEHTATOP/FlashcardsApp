using FlashcardsAppContracts.DTOs.Responses;
using FlashcardsBlazorUI.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlashcardsBlazorUI.Services;

public class GroupStore : IDisposable
{
    private readonly HttpClient _http;
    private readonly IGroupNotificationService _notificationService;
    private bool _disposed = false;

    public List<ResultGroupDto> Groups { get; private set; } = new();
    public event Action? GroupsChanged;
    
    public GroupStore(HttpClient http, IGroupNotificationService notificationService)
    {
        _http = http;
        _notificationService = notificationService;
        
        // Подписываемся на уведомления об изменении порядка групп
        _notificationService.OnGroupsReordered += HandleGroupsReordered;
        
        Console.WriteLine("GroupStore: Подписался на уведомления");
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

            Console.WriteLine("GroupStore: Загружаю данные из API...");
            var data = await _http.GetFromJsonAsync<List<ResultGroupDto>>("api/group", options);
            
            if (data is not null)
            {
                Groups = data;
                Console.WriteLine($"GroupStore: Загружено {Groups.Count} групп, уведомляю подписчиков");
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
        Console.WriteLine($"GroupStore: Получено уведомление от {sourceContainer}, обновляю данные");
        
        // Асинхронно обновляем данные
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
            Console.WriteLine("GroupStore: Отписался от уведомлений");
        }
    }
}