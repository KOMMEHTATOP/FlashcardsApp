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
    public static void NotifyGroupsReordered()
    {
        Console.WriteLine("JS уведомил об изменении порядка групп");
        
        if (_serviceScopeFactory == null)
        {
            Console.WriteLine("JSBridge не инициализирован");
            return;
        }

        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var groupOrderService = scope.ServiceProvider.GetRequiredService<IGroupOrderService>();
            groupOrderService.NotifyGroupsReordered();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка в NotifyGroupsReordered: {ex.Message}");
        }
    }

    [JSInvokable]
    public static void NotifyGroupDeleted()
    {
        Console.WriteLine("JS уведомил об удалении группы");
        
        if (_serviceScopeFactory == null)
        {
            Console.WriteLine("JSBridge не инициализирован");
            return;
        }

        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var groupOrderService = scope.ServiceProvider.GetRequiredService<IGroupOrderService>();
            groupOrderService.NotifyGroupsReordered();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка в NotifyGroupDeleted: {ex.Message}");
        }
    }
}
