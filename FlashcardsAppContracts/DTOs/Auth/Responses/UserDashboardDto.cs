using FlashcardsAppContracts.DTOs.Achievements.Responses;
using FlashcardsAppContracts.DTOs.Groups.Responses;
using FlashcardsAppContracts.DTOs.Statistics.Responses;

namespace FlashcardsAppContracts.DTOs.Auth.Responses;

public class UserDashboardDto
{
    public Guid Id { get; set; }
    public string? Login { get; set; }
    public string? Email { get; set; }
    
    public UserStatsDto? Statistics { get; set; }
    public List<ResultGroupDto> Groups { get; set; } = new();
    public List<AchievementWithStatusDto> Achievements { get; set; } = new();
}
