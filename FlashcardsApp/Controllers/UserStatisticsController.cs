using FlashcardsApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlashcardsApp.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserStatisticsController : ControllerBase
{
    private readonly UserStatisticsService _statisticsService;
    private readonly GamificationService _gamificationService;

    public UserStatisticsController(
        UserStatisticsService statisticsService,
        GamificationService gamificationService)
    {
        _statisticsService = statisticsService;
        _gamificationService = gamificationService;
    }

    // Один эндпоинт для получения статистики
    [HttpGet]
    public async Task<IActionResult> GetUserStats()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _statisticsService.GetUserStatsAsync(userId);

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
        var result = await _gamificationService.GetMotivationalMessageAsync(userId);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Data);
    }
}
