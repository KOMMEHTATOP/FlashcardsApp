using FlashcardsBlazorUI.Interfaces;
using FlashcardsBlazorUI.Services;
using System.Net.Http.Headers;

namespace FlashcardsBlazorUI.Helpers;

public class AuthenticationHandler : DelegatingHandler
{
    private readonly ITokenStorageService _tokenStorage;

    public AuthenticationHandler(ITokenStorageService tokenStorage)
    {
        _tokenStorage = tokenStorage;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _tokenStorage.GetToken();
    
        Console.WriteLine($"[AuthenticationHandler] Запрос к: {request.RequestUri}");
        Console.WriteLine($"[AuthenticationHandler] Токен: {(token != null ? token.Substring(0, Math.Min(20, token.Length)) + "..." : "NULL")}");
    
        if (!string.IsNullOrEmpty(token))
        {
            var isExpired = JwtHelper.IsTokenExpired(token);
            Console.WriteLine($"[AuthenticationHandler] Токен истек: {isExpired}");
        
            if (!isExpired)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Console.WriteLine($"[AuthenticationHandler] Authorization header добавлен");
            }
        }
        else
        {
            Console.WriteLine("[AuthenticationHandler] Токен не найден в storage");
        }

        return base.SendAsync(request, cancellationToken);
    }
    
    
    
}

