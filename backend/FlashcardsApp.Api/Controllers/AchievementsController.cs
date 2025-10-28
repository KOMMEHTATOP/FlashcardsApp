using FlashcardsApp.BLL.Interfaces.Achievements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlashcardsApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AchievementsController : ControllerBase
{
    private readonly IAchievementBL _achievementBL;
    private readonly IAchievementProgressBL _progressBl;
    private readonly IAchievementRecommendationBL _recommendationBl;
    private readonly ILogger<AchievementsController> _logger;

    public AchievementsController(
        IAchievementBL achievementBL,
        IAchievementProgressBL progressBl,
        IAchievementRecommendationBL recommendationBl,
        ILogger<AchievementsController> logger)
    {
        _achievementBL = achievementBL;
        _progressBl = progressBl;
        _recommendationBl = recommendationBl;
        _logger = logger;
    }


    /// <summary>
    /// Получить все существующие достижения в системе
    /// </summary>
    [HttpGet("all")]
    public async Task<IActionResult> GetAllAchievements()
    {
        var result = await _achievementBL.GetAllAchievementsAsync();

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to fetch all achievements: {Errors}", string.Join(", ", result.Errors));
            return BadRequest(new
            {
                errors = result.Errors
            });
        }

        return Ok(result.Data);
    }


    /// <summary>
    /// Получить прогресс конкретного достижения
    /// </summary>
    /// <param name="achievementId">ID достижения</param>
    [HttpGet("progress/{achievementId:guid}")]
    public async Task<IActionResult> GetAchievementProgress(Guid achievementId)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized(new
            {
                error = "User ID not found in token"
            });
        }

        var result = await _progressBl.CalculateAchievementProgressAsync(userId.Value, achievementId);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to get achievement progress for {AchievementId}: {Errors}",
                achievementId, string.Join(", ", result.Errors));

            // Если достижение не найдено - 404, иначе - 400
            if (result.Errors.Any(e => e.Contains("не найдено") || e.Contains("not found")))
            {
                return NotFound(new
                {
                    errors = result.Errors
                });
            }

            return BadRequest(new
            {
                errors = result.Errors
            });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Получить прогресс по всем достижениям
    /// </summary>
    [HttpGet("progress/all")]
    public async Task<IActionResult> GetAllAchievementsProgress()
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized(new
            {
                error = "User ID not found in token"
            });
        }

        var result = await _progressBl.GetAllAchievementsProgressAsync(userId.Value);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to get all achievements progress for user {UserId}: {Errors}",
                userId, string.Join(", ", result.Errors));
            return BadRequest(new
            {
                errors = result.Errors
            });
        }

        _logger.LogInformation("Retrieved progress for {Count} achievements for user {UserId}",
            result.Data?.Count() ?? 0, userId);

        return Ok(result.Data);
    }

    /// <summary>
    /// Получить рекомендации достижений (близкие к разблокировке)
    /// </summary>
    /// <param name="count">Количество рекомендаций (по умолчанию 3, максимум 10)</param>
    [HttpGet("recommendations")]
    public async Task<IActionResult> GetAchievementRecommendations([FromQuery] int count = 3)
    {
        // Валидация входных данных
        if (count <= 0 || count > 10)
        {
            return BadRequest(new
            {
                error = "Количество рекомендаций должно быть от 1 до 10"
            });
        }

        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized(new
            {
                error = "User ID not found in token"
            });
        }

        var result = await _recommendationBl.GetAchievementRecommendationsAsync(userId.Value, count);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to get achievement recommendations for user {UserId}: {Errors}",
                userId, string.Join(", ", result.Errors));
            return BadRequest(new
            {
                errors = result.Errors
            });
        }

        _logger.LogInformation("Retrieved {Count} recommendations for user {UserId}",
            result.Data?.Count() ?? 0, userId);

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
            return Unauthorized(new
            {
                error = "User ID not found in token"
            });
        }

        var result = await _achievementBL.GetUserAchievementsAsync(userId.Value);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to fetch achievements for user {UserId}: {Errors}",
                userId, string.Join(", ", result.Errors));
            return BadRequest(new
            {
                errors = result.Errors
            });
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
            return Unauthorized(new
            {
                error = "User ID not found in token"
            });
        }

        var result = await _achievementBL.GetAllAchievementsWithStatusAsync(userId.Value);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to fetch achievements with status for user {UserId}: {Errors}",
                userId, string.Join(", ", result.Errors));
            return BadRequest(new
            {
                errors = result.Errors
            });
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
            return Unauthorized(new
            {
                error = "User ID not found in token"
            });
        }

        var result = await _achievementBL.CheckAndUnlockAchievementsAsync(userId.Value);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to check achievements for user {UserId}: {Errors}",
                userId, string.Join(", ", result.Errors));
            return BadRequest(new
            {
                errors = result.Errors
            });
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
