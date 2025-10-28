using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.BLL.Interfaces.Achievements;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.DTOs.Achievements.Responses;
using FlashcardsApp.Models.DTOs.Auth.Responses;
using FlashcardsApp.Models.DTOs.Groups.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FlashcardsApp.BLL.Implementations;

public class UserBL : IUserBL
{
    private readonly UserManager<User> _userManager;
    private readonly IUserStatisticsBL _statisticsBl;
    private readonly IGroupBL _groupBl;
    private readonly IAchievementBL _achievementBL;
    private readonly ILogger<UserBL> _logger;

    public UserBL(
        UserManager<User> userManager,
        IUserStatisticsBL statisticsBl,
        IGroupBL groupBl,
        IAchievementBL achievementBL,
        ILogger<UserBL> logger)
    {
        _userManager = userManager;
        _statisticsBl = statisticsBl;
        _groupBl = groupBl;
        _achievementBL = achievementBL;
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

            var statisticsResult = await _statisticsBl.GetUserStatsAsync(userId);
            var groupsResult = await _groupBl.GetGroupsAsync(userId);
            var achievementsResult = await _achievementBL.GetAllAchievementsWithStatusAsync(userId);

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