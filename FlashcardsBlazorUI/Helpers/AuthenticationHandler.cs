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
        
        if (!string.IsNullOrEmpty(token))
        {
            var isExpired = JwtHelper.IsTokenExpired(token);
            
            if (!isExpired)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                Console.WriteLine("[AuthenticationHandler] Токен истек");
            }
        }
        else
        {
            Console.WriteLine("[AuthenticationHandler] Токен не найден в storage");
        }

        return base.SendAsync(request, cancellationToken);
    }
}

