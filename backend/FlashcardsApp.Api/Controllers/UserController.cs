using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : BaseController
{
    private readonly IUserBL _userBl;

    public UserController(IUserBL userBl, UserManager<User> userManager) : base(userManager)
    {
        _userBl = userBl;
    }

    /// <summary>
    /// Получить полный dashboard пользователя
    /// </summary>
    [HttpGet("me/dashboard")]
    public async Task<IActionResult> GetUserDashboard()
    {
        var userId = GetCurrentUserId();
        var result = await _userBl.GetUserDashboardAsync(userId);

        return OkOrNotFound(result);
    }
}
