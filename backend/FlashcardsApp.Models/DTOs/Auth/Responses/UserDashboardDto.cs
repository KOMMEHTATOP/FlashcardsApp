using FlashcardsApp.Models.DTOs.Achievements.Responses;
using FlashcardsApp.Models.DTOs.Groups.Responses;
using FlashcardsApp.Models.DTOs.Statistics.Responses;
using FlashcardsApp.Models.DTOs.Subscriptions.Responses;

namespace FlashcardsApp.Models.DTOs.Auth.Responses;

public class UserDashboardDto
{
    public Guid Id { get; set; }
    public string? Login { get; set; }
    public string? Email { get; set; }
    public int TotalSubscribers { get; set; }
    public string Role { get; set; } = string.Empty;
    public List<SubscribedGroupDto> MySubscriptions { get; set; }

    public UserStatsDto? Statistics { get; set; }
    public List<ResultGroupDto> Groups { get; set; } = new();
    public List<AchievementWithStatusDto> Achievements { get; set; } = new();
}
