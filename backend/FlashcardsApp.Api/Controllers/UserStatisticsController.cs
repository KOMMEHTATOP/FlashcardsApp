using FlashcardsApp.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlashcardsApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserStatisticsController : ControllerBase
{
    private readonly IUserStatisticsBL _statisticsBl;
    private readonly IGamificationBL _gamificationBl;

    public UserStatisticsController(
        IUserStatisticsBL statisticsBl,
        IGamificationBL gamificationBl)
    {
        _statisticsBl = statisticsBl;
        _gamificationBl = gamificationBl;
    }

    // Один эндпоинт для получения статистики
    [HttpGet]
    public async Task<IActionResult> GetUserStats()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _statisticsBl.GetUserStatsAsync(userId);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Data);
    }

    [HttpGet("motivational-message")]
    public async Task<IActionResult> GetMotivationalMessage()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _gamificationBl.GetMotivationalMessageAsync(userId);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Data);
    }
}
