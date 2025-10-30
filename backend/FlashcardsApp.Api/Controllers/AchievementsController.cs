using FlashcardsApp.BLL.Interfaces.Achievements;
using FlashcardsApp.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AchievementsController : BaseController
{
    private readonly IAchievementBL _achievementBL;
    private readonly IAchievementProgressBL _progressBl;
    private readonly IAchievementRecommendationBL _recommendationBl;

    public AchievementsController(
        IAchievementBL achievementBL,
        IAchievementProgressBL progressBl,
        IAchievementRecommendationBL recommendationBl,
        UserManager<User> userManager) : base(userManager)
    {
        _achievementBL = achievementBL;
        _progressBl = progressBl;
        _recommendationBl = recommendationBl;
    }

    /// <summary>
    /// Получить все существующие достижения в системе
    /// </summary>
    [HttpGet("all")]
    public async Task<IActionResult> GetAllAchievements()
    {
        var result = await _achievementBL.GetAllAchievementsAsync();
        return OkOrBadRequest(result);
    }

    /// <summary>
    /// Получить прогресс конкретного достижения
    /// </summary>
    /// <param name="achievementId">ID достижения</param>
    [HttpGet("progress/{achievementId:guid}")]
    public async Task<IActionResult> GetAchievementProgress(Guid achievementId)
    {
        var userId = GetCurrentUserId();
        var result = await _progressBl.CalculateAchievementProgressAsync(userId, achievementId);
        return OkOrNotFound(result);
    }

    /// <summary>
    /// Получить прогресс по всем достижениям
    /// </summary>
    [HttpGet("progress/all")]
    public async Task<IActionResult> GetAllAchievementsProgress()
    {
        var userId = GetCurrentUserId();
        var result = await _progressBl.GetAllAchievementsProgressAsync(userId);
        return OkOrBadRequest(result);
    }

    /// <summary>
    /// Получить рекомендации достижений (близкие к разблокировке)
    /// </summary>
    /// <param name="count">Количество рекомендаций (по умолчанию 3, максимум 10)</param>
    [HttpGet("recommendations")]
    public async Task<IActionResult> GetAchievementRecommendations([FromQuery] int count = 3)
    {
        if (count <= 0 || count > 10)
        {
            return BadRequest(new
            {
                error = "Количество рекомендаций должно быть от 1 до 10"
            });
        }

        var userId = GetCurrentUserId();
        var result = await _recommendationBl.GetAchievementRecommendationsAsync(userId, count);
        return OkOrBadRequest(result);
    }

    /// <summary>
    /// Получить разблокированные достижения текущего пользователя
    /// </summary>
    [HttpGet("my")]
    public async Task<IActionResult> GetMyAchievements()
    {
        var userId = GetCurrentUserId();
        var result = await _achievementBL.GetUserAchievementsAsync(userId);
        return OkOrBadRequest(result);
    }

    /// <summary>
    /// Получить все достижения со статусом разблокировки для текущего пользователя
    /// </summary>
    [HttpGet("all-with-status")]
    public async Task<IActionResult> GetAllAchievementsWithStatus()
    {
        var userId = GetCurrentUserId();
        var result = await _achievementBL.GetAllAchievementsWithStatusAsync(userId);
        return OkOrBadRequest(result);
    }

    /// <summary>
    /// Проверить статистику пользователя и автоматически разблокировать новые достижения
    /// </summary>
    [HttpPost("check-and-unlock")]
    public async Task<IActionResult> CheckAndUnlockAchievements()
    {
        var userId = GetCurrentUserId();
        var result = await _achievementBL.CheckAndUnlockAchievementsAsync(userId);
        return OkOrBadRequest(result);
    }
}