using FlashcardsApp.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlashcardsApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserBL _userBl;

    public UserController(IUserBL userBl)
    {
        _userBl = userBl;
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

        var result = await _userBl.GetUserDashboardAsync(userId);
        
        if (!result.IsSuccess)
        {
            return NotFound(new { message = string.Join(", ", result.Errors) });
        }

        return Ok(result.Data);
    }

}