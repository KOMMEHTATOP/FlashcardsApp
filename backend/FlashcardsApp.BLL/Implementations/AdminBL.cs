using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.DAL;
using FlashcardsApp.Models.DTOs.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FlashcardsApp.BLL.Implementations;

public class AdminBL : IAdminBL
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AdminBL> _logger;

    public AdminBL(ApplicationDbContext context, ILogger<AdminBL> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<AdminUserDto>> GetAllUsersAsync()
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

            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching users list in AdminBL");
            throw; 
        }
    }
}
