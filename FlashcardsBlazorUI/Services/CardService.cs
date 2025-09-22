using FlashcardsAppContracts.DTOs.Requests;
using FlashcardsBlazorUI.Models;
using System.Text.Json;

namespace FlashcardsBlazorUI.Services
{
    public class CardService : BaseApiService
    {
        public CardService(IHttpClientFactory httpClientFactory) 
            : base(httpClientFactory)
        {
        }

        public async Task<Card?> GetCardAsync(Guid cardId)
        {
            var response = await GetAsync($"api/cards/{cardId}");
            if (!response.IsSuccessStatusCode) return null;
            
            return await response.Content.ReadFromJsonAsync<Card>();
        }

        public async Task<List<Card>> GetCardsAsync(Guid groupId)
        {
            var response = await GetAsync($"api/groups/{groupId}/cards");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<List<Card>>() ?? new();
        }

        public async Task<Card> CreateCardAsync(CreateCardDto cardDto, Guid groupId)
        {
            var response = await PostAsJsonAsync($"api/groups/{groupId}/cards", cardDto);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Card>(responseJson, _jsonOptions)!;
        }

        public async Task<List<CardRating>> GetCardRatingsAsync(Guid cardId)
        {
            var response = await GetAsync($"api/cards/{cardId}/ratings");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CardRating>>(json, _jsonOptions) ?? new List<CardRating>();
        }

        public async Task<CardRating> RateCardAsync(Guid cardId, int rating)
        {
            var ratingData = new { rating };
            var response = await PostAsJsonAsync($"api/cards/{cardId}/ratings", ratingData);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CardRating>(responseJson, _jsonOptions)!;
        }
    }
}