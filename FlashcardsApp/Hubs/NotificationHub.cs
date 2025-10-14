using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace FlashcardsApp.Hubs;

/// <summary>
/// SignalR Hub для real-time уведомлений
/// Клиенты подключаются к этому Hub'у для получения уведомлений в реальном времени
/// </summary>
[Authorize] // Только авторизованные пользователи могут подключаться
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Вызывается при подключении клиента
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!string.IsNullOrEmpty(userId))
        {
            // Добавляем пользователя в его персональную группу
            // Это позволяет отправлять уведомления конкретному пользователю
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            
            _logger.LogInformation(
                "User {UserId} connected to NotificationHub with ConnectionId {ConnectionId}",
                userId, Context.ConnectionId);
        }

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Вызывается при отключении клиента
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
            
            _logger.LogInformation(
                "User {UserId} disconnected from NotificationHub. ConnectionId: {ConnectionId}",
                userId, Context.ConnectionId);
        }

        if (exception != null)
        {
            _logger.LogError(exception, "User disconnected with error");
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Ping-метод для проверки соединения (опционально)
    /// Клиент может вызывать его для проверки активности соединения
    /// </summary>
    public async Task Ping()
    {
        await Clients.Caller.SendAsync("Pong");
    }
}