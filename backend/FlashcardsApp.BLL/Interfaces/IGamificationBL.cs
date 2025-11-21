using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.Models.DTOs.Statistics.Responses;

namespace FlashcardsApp.BLL.Interfaces;

public interface IGamificationBL
{
    Task<int> CalculateXPForCardAsync(Guid userId, Guid cardId, int rating);
    Task<ServiceResult<(bool leveledUp, int newLevel)>> AddXPToUserAsync(Guid userId, int xp);
    Task<ServiceResult<bool>> UpdateStreakAsync(Guid userId);
    int CalculateXPForLevel(int level);
    Task<ServiceResult<MotivationalMessageDto>> GetMotivationalMessageAsync(Guid userId);
}
