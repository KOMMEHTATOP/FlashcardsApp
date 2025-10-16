using FlashcardsApp.Interfaces;
using FlashcardsApp.Interfaces.Achievements;
using FlashcardsApp.Models;
using FlashcardsAppContracts.DTOs.Groups.Responses;
using FlashcardsAppContracts.DTOs.Achievements.Responses;
using FlashcardsAppContracts.DTOs.Auth.Responses;
using Microsoft.AspNetCore.Identity;

namespace FlashcardsApp.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IUserStatisticsService _statisticsService;
    private readonly IGroupService _groupService;
    private readonly IAchievementService _achievementService;
    private readonly ILogger<UserService> _logger;

    public UserService(
        UserManager<User> userManager,
        IUserStatisticsService statisticsService,
        IGroupService groupService,
        IAchievementService achievementService,
        ILogger<UserService> logger)
    {
        _userManager = userManager;
        _statisticsService = statisticsService;
        _groupService = groupService;
        _achievementService = achievementService;
        _logger = logger;
    }

    public async Task<ServiceResult<UserDashboardDto>> GetUserDashboardAsync(Guid userId)
    {
        try
        {
            _logger.LogInformation("Fetching dashboard for user {UserId}", userId);
            
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", userId);
                return ServiceResult<UserDashboardDto>.Failure("Пользователь не найден");
            }

            var statisticsResult = await _statisticsService.GetUserStatsAsync(userId);
            var groupsResult = await _groupService.GetGroupsAsync(userId);
            var achievementsResult = await _achievementService.GetAllAchievementsWithStatusAsync(userId);

            // Агрегируем результаты
            var dashboard = new UserDashboardDto
            {
                Id = user.Id,
                Login = user.Login,
                Email = user.Email,
                Statistics = statisticsResult.IsSuccess ? statisticsResult.Data : null,
                Groups = groupsResult.IsSuccess 
                    ? groupsResult.Data!.ToList() 
                    : new List<ResultGroupDto>(),
                Achievements = achievementsResult.IsSuccess 
                    ? achievementsResult.Data!.ToList() 
                    : new List<AchievementWithStatusDto>()
            };

            _logger.LogInformation("Dashboard successfully loaded for user {UserId}", userId);
            return ServiceResult<UserDashboardDto>.Success(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard for user {UserId}", userId);
            return ServiceResult<UserDashboardDto>.Failure("Ошибка при загрузке данных профиля");
        }
    }
    
}