using FlashcardsApp.DAL;
using FlashcardsApp.Models.DTOs.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] 
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AdminController> _logger;

    public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Получить всех пользователей
    /// </summary>
    [HttpGet("users")]
    public async Task<ActionResult<List<AdminUserDto>>> GetAllUsers()
    {
        try
        {
            var users = await _context.Users
                .AsNoTracking() 
                .Select(u => new AdminUserDto
                {
                    Id = u.Id,
                    Login = u.Login,
                    Email = u.Email ?? "No Email",
                    Role = u.Role, 
                    TotalRating = u.TotalRating,
                    CreatedAt = u.CreatedAt,
                    LastLogin = u.LastLogin,
                    GroupsCount = u.Groups != null ? u.Groups.Count : 0,
                    CardsCount = u.Groups != null 
                        ? u.Groups.SelectMany(g => g.Cards).Count() 
                        : 0
                })
                .OrderByDescending(u => u.CreatedAt) 
                .ToListAsync();

            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching users list for admin panel");
            return StatusCode(500, "Internal server error");
        }
    }
}