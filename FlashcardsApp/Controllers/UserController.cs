using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FlashcardsApp.Interfaces;

namespace FlashcardsApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(
        IUserService userService,
        ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Получить полный dashboard пользователя
    /// </summary>
    [HttpGet("me/dashboard")]
    [Authorize]
    public async Task<IActionResult> GetUserDashboard()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Пользователь не аутентифицирован" });
        }

        var result = await _userService.GetUserDashboardAsync(userId);
        
        if (!result.IsSuccess)
        {
            return NotFound(new { message = string.Join(", ", result.Errors) });
        }

        return Ok(result.Data);
    }

}