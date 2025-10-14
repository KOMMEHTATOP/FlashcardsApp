using FlashcardsApp.Interfaces.Achievements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlashcardsApp.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AchievementsController : ControllerBase
{
    private readonly IAchievementService _achievementService;
    private readonly ILogger<AchievementsController> _logger;

    public AchievementsController(
        IAchievementService achievementService,
        ILogger<AchievementsController> logger)
    {
        _achievementService = achievementService;
        _logger = logger;
    }

    /// <summary>
    /// Получить все существующие достижения в системе
    /// </summary>
    [HttpGet("all")]
    public async Task<IActionResult> GetAllAchievements()
    {
        var result = await _achievementService.GetAllAchievementsAsync();

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to fetch all achievements: {Errors}", string.Join(", ", result.Errors));
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Получить разблокированные достижения текущего пользователя
    /// </summary>
    [HttpGet("my")]
    public async Task<IActionResult> GetMyAchievements()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(new { error = "User ID not found in token" });
        }

        var result = await _achievementService.GetUserAchievementsAsync(userId.Value);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to fetch achievements for user {UserId}: {Errors}", 
                userId, string.Join(", ", result.Errors));
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Получить все достижения со статусом разблокировки для текущего пользователя
    /// </summary>
    [HttpGet("all-with-status")]
    public async Task<IActionResult> GetAllAchievementsWithStatus()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(new { error = "User ID not found in token" });
        }

        var result = await _achievementService.GetAllAchievementsWithStatusAsync(userId.Value);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to fetch achievements with status for user {UserId}: {Errors}", 
                userId, string.Join(", ", result.Errors));
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Проверить статистику пользователя и автоматически разблокировать новые достижения
    /// </summary>
    [HttpPost("check-and-unlock")]
    public async Task<IActionResult> CheckAndUnlockAchievements()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(new { error = "User ID not found in token" });
        }

        var result = await _achievementService.CheckAndUnlockAchievementsAsync(userId.Value);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to check achievements for user {UserId}: {Errors}", 
                userId, string.Join(", ", result.Errors));
            return BadRequest(new { errors = result.Errors });
        }

        _logger.LogInformation("User {UserId} unlocked {Count} new achievements", 
            userId, result.Data?.Count ?? 0);

        return Ok(result.Data);
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userIdClaim))
        {
            _logger.LogWarning("User ID claim not found in token");
            return null;
        }

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        _logger.LogError("Invalid User ID format in token: {UserIdClaim}", userIdClaim);
        return null;
    }
}