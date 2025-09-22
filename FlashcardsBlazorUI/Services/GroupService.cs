using FlashcardsAppContracts.DTOs.Requests;
using FlashcardsAppContracts.DTOs.Responses;
using System.Text.Json;

namespace FlashcardsBlazorUI.Services
{
    public class GroupService : BaseApiService
    {
        public GroupService(IHttpClientFactory httpClientFactory) 
            : base(httpClientFactory)
        {
        }

        public async Task<List<ResultGroupDto>> GetGroupsAsync()
        {
            var response = await GetAsync("api/group");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ResultGroupDto>>(json, _jsonOptions) ?? new List<ResultGroupDto>();
        }

        public async Task<ResultGroupDto?> GetGroupAsync(Guid groupId)
        {
            var response = await GetAsync($"api/group/{groupId}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ResultGroupDto>(json, _jsonOptions);
        }

        public async Task<ResultGroupDto> CreateGroupAsync(CreateGroupDto groupDto)
        {
            var response = await PostAsJsonAsync("api/group", groupDto);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ResultGroupDto>(responseJson, _jsonOptions)!;
        }

        public async Task<bool> UpdateGroupOrderAsync(List<ReorderGroupDto> groupOrders)
        {
            try
            {
                var response = await PutAsJsonAsync("api/group/reorder", groupOrders);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteGroupAsync(Guid groupId)
        {
            try
            {
                var response = await DeleteAsync($"api/group/{groupId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка удаления группы: {ex.Message}");
                return false;
            }
        }
    }
}