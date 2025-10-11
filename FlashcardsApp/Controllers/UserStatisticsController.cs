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

    public UserStatisticsController(UserStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyStatistics()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _statisticsService.GetUserStatisticsAsync(userId);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Data);
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateStatistics([FromBody] UpdateStatisticsRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _statisticsService.UpdateStatisticsAsync(userId, request.XpGained, request.StudyTime);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Data);
    }
}

public class UpdateStatisticsRequest
{
    public int XpGained { get; set; }
    public TimeSpan StudyTime { get; set; }
}
