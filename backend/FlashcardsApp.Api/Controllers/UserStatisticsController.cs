using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserStatisticsController : BaseController
{
    private readonly IUserStatisticsBL _statisticsBl;
    private readonly IGamificationBL _gamificationBl;

    public UserStatisticsController(
        IUserStatisticsBL statisticsBl, 
        IGamificationBL gamificationBl, 
        UserManager<User> userManager) : base(userManager)
    {
        _statisticsBl = statisticsBl;
        _gamificationBl = gamificationBl;
    }

    /// <summary>
    /// Получить статистику пользователя
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetUserStats()
    {
        var userId = GetCurrentUserId();
        var result = await _statisticsBl.GetUserStatsAsync(userId);

        return OkOrBadRequest(result);
    }

    /// <summary>
    /// Получить мотивационное сообщение для пользователя
    /// </summary>
    [HttpGet("motivational-message")]
    public async Task<IActionResult> GetMotivationalMessage()
    {
        var userId = GetCurrentUserId();
        var result = await _gamificationBl.GetMotivationalMessageAsync(userId);

        return OkOrBadRequest(result);
    }
}
