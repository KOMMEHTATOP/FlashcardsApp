using FlashcardsApp.Models.DTOs.Achievements.Responses;
using FlashcardsApp.Models.DTOs.Groups.Responses;
using FlashcardsApp.Models.DTOs.Statistics.Responses;

namespace FlashcardsApp.Models.DTOs.Auth.Responses;

public class UserDashboardDto
{
    public Guid Id { get; set; }
    public string? Login { get; set; }
    public string? Email { get; set; }
    
    public UserStatsDto? Statistics { get; set; }
    public List<ResultGroupDto> Groups { get; set; } = new();
    public List<AchievementWithStatusDto> Achievements { get; set; } = new();
}
