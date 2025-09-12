using FlashcardsApp.Data;
using FlashcardsApp.Mapping;
using FlashcardsApp.Models;
using FlashcardsApp.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FlashcardsApp.Services;

public class GroupService
{
    private readonly ApplicationDbContext _context;

    public GroupService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<ResultGroupDto>> GetGroupByIdAsync(Guid groupId, Guid userId)
    {
        var group = await _context.Groups
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == groupId && g.UserId == userId);

        if (group == null)
        {
            return ServiceResult<ResultGroupDto>.Failure("Group not found or access denied");
        }

        return ServiceResult<ResultGroupDto>.Success(group.ToDto());
    }

    public async Task<ServiceResult<IEnumerable<ResultGroupDto>>> GetGroupsAsync(Guid userId)
    {
        var groups = await _context.Groups
            .AsNoTracking()
            .Where(g => g.UserId == userId)
            .OrderBy(g => g.CreatedAt)
            .ToListAsync();

        var groupDtos = groups.Select(g => g.ToDto());
        return ServiceResult<IEnumerable<ResultGroupDto>>.Success(groupDtos);
    }

    public async Task<ServiceResult<ResultGroupDto>> CreateNewGroupAsync(CreateGroupDto model, Guid userId)
    {
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

        return ServiceResult<ResultGroupDto>.Success(group.ToDto());
    }

    public async Task<ServiceResult<ResultGroupDto>> UpdateGroupAsync(Guid groupId, Guid userId, CreateGroupDto model)
    {
        var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == groupId && g.UserId == userId);

        if (group == null)
        {
            return ServiceResult<ResultGroupDto>.Failure("Group not found or access denied");
        }

        if (group.GroupName != model.Name)
        {
            var nameExists = await _context.Groups.AnyAsync(g => 
                g.GroupName == model.Name && 
                g.UserId == userId && 
                g.Id != groupId);

            if (nameExists)
            {
                return ServiceResult<ResultGroupDto>.Failure("Group with the same name already exists");
            }
        }

        group.GroupName = model.Name;
        group.GroupColor = model.Color;

        await _context.SaveChangesAsync();
        return ServiceResult<ResultGroupDto>.Success(group.ToDto());
    }

    public async Task<ServiceResult<bool>> DeleteGroupAsync(Guid groupId, Guid userId)
    {
        var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == groupId && g.UserId == userId);

        if (group == null)
        {
            return ServiceResult<bool>.Failure("Group not found or access denied");
        }

        _context.Groups.Remove(group);
        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Success(true);
    }

    private bool IsUniqueConstraintViolation(DbUpdateException exception)
    {
        if (exception.InnerException is PostgresException postgresEx)
        {
            return postgresEx.SqlState == "23505";
        }
        return false;
    }
}