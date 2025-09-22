using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlashcardsBlazorUI.Services
{
    public abstract class BaseApiService
    {
        protected readonly HttpClient _httpClient;
        protected readonly JsonSerializerOptions _jsonOptions;

        protected BaseApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("FlashcardsAPI");
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() } 
            };
        }

        // Общий метод для обработки HTTP запросов
        protected async Task<HttpResponseMessage> SendRequestAsync(Func<Task<HttpResponseMessage>> request)
        {
            var response = await request();
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("Требуется авторизация");
            }
            
            return response;
        }

        // HTTP методы - токен автоматически добавляется через AuthenticationHandler
        protected async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return await SendRequestAsync(() => _httpClient.GetAsync(requestUri));
        }

        protected async Task<HttpResponseMessage> PostAsJsonAsync<T>(string requestUri, T value)
        {
            return await SendRequestAsync(() => _httpClient.PostAsJsonAsync(requestUri, value));
        }

        protected async Task<HttpResponseMessage> PutAsJsonAsync<T>(string requestUri, T value)
        {
            return await SendRequestAsync(() => _httpClient.PutAsJsonAsync(requestUri, value));
        }

        protected async Task<HttpResponseMessage> DeleteAsync(string requestUri)
        {
            return await SendRequestAsync(() => _httpClient.DeleteAsync(requestUri));
        }
    }
}
