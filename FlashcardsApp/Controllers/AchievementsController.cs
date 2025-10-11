using FlashcardsApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlashcardsApp.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AchievementsController : ControllerBase
{
    private readonly AchievementService _achievementService;

    public AchievementsController(AchievementService achievementService)
    {
        _achievementService = achievementService;
    }

    // Получить все существующие достижения
    [HttpGet("all")]
    public async Task<IActionResult> GetAllAchievements()
    {
        var result = await _achievementService.GetAllAchievementsAsync();
        return Ok(result.Data);
    }

    // Получить достижения текущего пользователя
    [HttpGet("my")]
    public async Task<IActionResult> GetMyAchievements()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _achievementService.GetUserAchievementsAsync(userId);
        return Ok(result.Data);
    }

    // Проверить и разблокировать новые достижения
    [HttpPost("check")]
    public async Task<IActionResult> CheckAchievements()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _achievementService.CheckAndUnlockAchievementsAsync(userId);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(new
        {
            newlyUnlocked = result.Data,
            count = result.Data.Count
        });
    }
}
