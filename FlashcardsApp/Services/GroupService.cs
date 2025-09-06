using FlashcardsApp.Data;
using FlashcardsApp.Mapping;
using FlashcardsApp.Models;
using FlashcardsApp.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FlashcardsApp.Services;

public class GroupService
{
    private readonly UserManager<User> _userManager;
    private ApplicationDbContext _context;

    public GroupService(UserManager<User> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<ServiceResult<ResultGroupDto>> GetGroupAsync(string groupName, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(groupName))
        {
            return ServiceResult<ResultGroupDto>.Failure("Empty group name");
        }

        var group = await _context.Groups
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.GroupName == groupName && g.UserId == userId);

        if (group == null)
        {
            return ServiceResult<ResultGroupDto>.Failure("Group not found or access denied");
        }

        return ServiceResult<ResultGroupDto>.Success(group.ToDto());
    }

    public async Task<ServiceResult<ResultGroupDto>> CreateNewGroupAsync(CreateGroupDto model, Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            return ServiceResult<ResultGroupDto>.Failure("User not found");
        }

        var group = new Group()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            GroupName = model.Name,
            GroupColor = model.Color,
            CreatedAt = DateTime.UtcNow,
        };

        _context.Groups.Add(group);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            return ServiceResult<ResultGroupDto>.Failure("You already have a group with this name");
        }

        var resultDto = group.ToDto();

        return ServiceResult<ResultGroupDto>.Success(resultDto);
    }

    private async Task<ServiceResult<User>> EnsureUserExists(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user is not null
            ? ServiceResult<User>.Success(user)
            : ServiceResult<User>.Failure("User not found");
    }
    
    private bool IsUniqueConstraintViolation(DbUpdateException exception)
    {
        // InnerException будет именно PostgresException при нарушении ограничения
        if (exception.InnerException is PostgresException postgresEx)
        {
            // 23505 = unique_violation
            return postgresEx.SqlState == "23505";
        }

        return false;
    }
}
