using System.Net.Http.Headers;

namespace FlashcardsBlazorUI.Services
{
    public class AuthenticationHandler : DelegatingHandler
    {
        private readonly ITokenManager _tokenManager;

        public AuthenticationHandler(ITokenManager tokenManager)
        {
            _tokenManager = tokenManager;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            Console.WriteLine($"[AuthHandler] Токен: {_tokenManager.Token ?? "NULL"}");
    
            if (!string.IsNullOrEmpty(_tokenManager.Token))
            {
                request.Headers.Authorization = 
                    new AuthenticationHeaderValue("Bearer", _tokenManager.Token);
                Console.WriteLine("[AuthHandler] Токен добавлен в заголовки");
            }
            else
            {
                Console.WriteLine("[AuthHandler] Токен пустой, заголовки не добавлены");
            }

            return base.SendAsync(request, cancellationToken);
        }    }
}
