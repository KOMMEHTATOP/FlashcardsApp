using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.BLL.Interfaces.Achievements;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.DTOs.Achievements.Responses;
using FlashcardsApp.Models.DTOs.Auth.Responses;
using FlashcardsApp.Models.DTOs.Groups.Responses;
using FlashcardsApp.Models.DTOs.Subscriptions.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FlashcardsApp.BLL.Implementations;

public class UserBL : IUserBL
{
    private readonly UserManager<User> _userManager;
    private readonly IUserStatisticsBL _statisticsBl;
    private readonly IGroupBL _groupBl;
    private readonly IAchievementBL _achievementBL;
    private readonly ISubscriptionBL _subscriptionBL;
    private readonly ILogger<UserBL> _logger;

    public UserBL(
        UserManager<User> userManager,
        IUserStatisticsBL statisticsBl,
        IGroupBL groupBl,
        IAchievementBL achievementBL,
        ISubscriptionBL subscriptionBL,
        ILogger<UserBL> logger)
    {
        _userManager = userManager;
        _statisticsBl = statisticsBl;
        _groupBl = groupBl;
        _achievementBL = achievementBL;
        _subscriptionBL = subscriptionBL;
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

            // Выполняем запросы ПОСЛЕДОВАТЕЛЬНО, не параллельно
            var statisticsResult = await _statisticsBl.GetUserStatsAsync(userId);
            var groupsResult = await _groupBl.GetGroupsAsync(userId);
            var achievementsResult = await _achievementBL.GetAllAchievementsWithStatusAsync(userId);
            var subscriptionsResult = await _subscriptionBL.GetSubscribedGroupsAsync(userId);

            // Считаем общее количество подписчиков
            var totalSubscribers = 0;

            if (groupsResult.IsSuccess && groupsResult.Data != null)
            {
                totalSubscribers = groupsResult.Data.Sum(g => g.SubscriberCount);
            }

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
                    : new List<AchievementWithStatusDto>(),
                TotalSubscribers = totalSubscribers,
                MySubscriptions = subscriptionsResult.IsSuccess
                    ? subscriptionsResult.Data!.ToList()
                    : new List<SubscribedGroupDto>()
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
