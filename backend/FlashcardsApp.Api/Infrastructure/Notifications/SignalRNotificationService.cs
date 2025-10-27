using FlashcardsApp.Api.Hubs;
using FlashcardsApp.Models.Notifications;
using FlashcardsApp.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace FlashcardsApp.Api.Infrastructure.Notifications;

/// <summary>
/// Реализация сервиса уведомлений через SignalR
/// Инкапсулирует логику работы с IHubContext
/// </summary>
public class SignalRNotificationService : INotificationService
{ 
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<SignalRNotificationService> _logger;

    public SignalRNotificationService(
        IHubContext<NotificationHub> hubContext,
        ILogger<SignalRNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    /// <summary>
    /// Отправить уведомление о разблокировке достижения
    /// </summary>
    public async Task SendAchievementUnlockedAsync(Guid userId, AchievementUnlockedNotification notification)
    {
        try
        {
            // Отправляем уведомление в группу пользователя
            // Все подключения этого пользователя получат уведомление
            await _hubContext.Clients
                .Group($"user_{userId}")
                .SendAsync("AchievementUnlocked", notification);

            _logger.LogInformation(
                "Achievement notification sent to user {UserId}: {AchievementName}",
                userId, notification.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Failed to send achievement notification to user {UserId}", userId);
            
            // НЕ пробрасываем исключение дальше
            // Уведомление - это не критичная операция
            // Если пользователь офлайн, просто логируем ошибку
        }
    }

    /// <summary>
    /// Отправить уведомления о разблокировке нескольких достижений
    /// </summary>
    public async Task SendMultipleAchievementsUnlockedAsync(
        Guid userId, 
        List<AchievementUnlockedNotification> notifications)
    {
        try
        {
            // Отправляем массовое уведомление
            await _hubContext.Clients
                .Group($"user_{userId}")
                .SendAsync("MultipleAchievementsUnlocked", notifications);

            _logger.LogInformation(
                "Multiple achievements notification sent to user {UserId}: {Count} achievements",
                userId, notifications.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Failed to send multiple achievements notification to user {UserId}", userId);
        }
    }
}