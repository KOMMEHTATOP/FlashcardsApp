using FlashcardsBlazorUI.Interfaces;
using Microsoft.JSInterop;

namespace FlashcardsBlazorUI.Services;

public static class JSBridge
{
    private static IServiceScopeFactory? _serviceScopeFactory;
    
    public static void Initialize(IServiceProvider serviceProvider)
    {
        _serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
    }
    
    [JSInvokable]
    public static void NotifyGroupsReordered(string sourceContainer = "unknown")
    {
        if (_serviceScopeFactory == null)
        {
            return;
        }

        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<IGroupNotificationService>();
            notificationService.NotifyGroupsReordered(sourceContainer);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка в NotifyGroupsReordered: {ex.Message}");
        }
    }

    [JSInvokable]
    public static void NotifyGroupDeleted()
    {
        if (_serviceScopeFactory == null)
        {
            return;
        }

        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<IGroupNotificationService>();
            notificationService.NotifyGroupsReordered("delete");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка в NotifyGroupDeleted: {ex.Message}");
        }
    }
}
