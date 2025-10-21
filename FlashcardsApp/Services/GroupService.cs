using FlashcardsApp.Data;
using FlashcardsApp.Interfaces;
using FlashcardsApp.Mapping;
using FlashcardsApp.Models;
using FlashcardsAppContracts.DTOs.Groups.Requests;
using FlashcardsAppContracts.DTOs.Groups.Responses;
using FlashcardsAppContracts.DTOs.Requests;
using FlashcardsAppContracts.DTOs.Responses;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FlashcardsApp.Services;

public class GroupService : IGroupService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GroupService> _logger;

    public GroupService(
        ApplicationDbContext context,
        ILogger<GroupService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResult<ResultGroupDto>> GetGroupByIdAsync(Guid groupId, Guid userId)
    {
        _logger.LogInformation("Fetching group {GroupId} for user {UserId}", groupId, userId);

        var group = await _context.Groups
            .Include(g => g.Cards)
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == groupId && g.UserId == userId);

        if (group == null)
        {
            _logger.LogWarning("Group {GroupId} not found for user {UserId}", groupId, userId);
            return ServiceResult<ResultGroupDto>.Failure("Group not found or access denied");
        }

        _logger.LogDebug("Group {GroupId} successfully retrieved", groupId);
        return ServiceResult<ResultGroupDto>.Success(group.ToDto());
    }

    public async Task<ServiceResult<IEnumerable<ResultGroupDto>>> GetGroupsAsync(Guid userId)
    {
        _logger.LogInformation("Fetching all groups for user {UserId}", userId);

        var groups = await _context.Groups
            .Include(c => c.Cards)
            .AsNoTracking()
            .Where(g => g.UserId == userId)
            .OrderBy(g => g.Order)
            .ThenBy(g => g.CreatedAt)
            .ToListAsync();

        _logger.LogDebug("Retrieved {Count} groups for user {UserId}", groups.Count, userId);

        var groupDtos = groups.Select(g => g.ToDto());
        return ServiceResult<IEnumerable<ResultGroupDto>>.Success(groupDtos);
    }

    public async Task<ServiceResult<ResultGroupDto>> CreateNewGroupAsync(CreateGroupDto model, Guid userId)
    {
        _logger.LogInformation("Creating new group '{GroupName}' for user {UserId}", model.Name, userId);

        var group = new Group
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            GroupName = model.Name,
            GroupIcon = model.GroupIcon,
            GroupColor = model.Color,
            CreatedAt = DateTime.UtcNow,
            Order = 0
        };

        _context.Groups.Add(group);

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Group {GroupId} successfully created", group.Id);
            return ServiceResult<ResultGroupDto>.Success(group.ToDto());
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            _logger.LogWarning("Duplicate group name '{GroupName}' for user {UserId}", model.Name, userId);
            return ServiceResult<ResultGroupDto>.Failure("You already have a group with this name");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating group '{GroupName}' for user {UserId}", model.Name, userId);
            return ServiceResult<ResultGroupDto>.Failure("Failed to create group");
        }
    }

    public async Task<ServiceResult<ResultGroupDto>> UpdateGroupAsync(Guid groupId, Guid userId, CreateGroupDto model)
    {
        _logger.LogInformation("Updating group {GroupId} for user {UserId}", groupId, userId);

        var group = await GetUserGroupAsync(groupId, userId);
        if (group == null)
        {
            return ServiceResult<ResultGroupDto>.Failure("Group not found or access denied");
        }

        // Проверка уникальности имени только если имя изменилось
        if (group.GroupName != model.Name)
        {
            var nameExists = await _context.Groups.AnyAsync(g =>
                g.GroupName == model.Name &&
                g.UserId == userId &&
                g.Id != groupId);

            if (nameExists)
            {
                _logger.LogWarning("Group name '{GroupName}' already exists for user {UserId}", model.Name, userId);
                return ServiceResult<ResultGroupDto>.Failure("Group with the same name already exists");
            }
        }

        group.GroupName = model.Name;
        group.GroupColor = model.Color;
        group.GroupIcon = model.GroupIcon;

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Group {GroupId} successfully updated", groupId);
            return ServiceResult<ResultGroupDto>.Success(group.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating group {GroupId}", groupId);
            return ServiceResult<ResultGroupDto>.Failure("Failed to update group");
        }
    }

    public async Task<ServiceResult<bool>> DeleteGroupAsync(Guid groupId, Guid userId)
    {
        _logger.LogInformation("Deleting group {GroupId} for user {UserId}", groupId, userId);

        var group = await GetUserGroupAsync(groupId, userId);
        if (group == null)
        {
            return ServiceResult<bool>.Failure("Group not found or access denied");
        }

        _context.Groups.Remove(group);

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Group {GroupId} successfully deleted", groupId);
            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting group {GroupId}", groupId);
            return ServiceResult<bool>.Failure("Failed to delete group");
        }
    }

    public async Task<ServiceResult<bool>> UpdateGroupsOrderAsync(List<ReorderGroupDto> groupOrders, Guid userId)
    {
        _logger.LogInformation("Updating order for {Count} groups for user {UserId}", groupOrders.Count, userId);

        try
        {
            var groupIds = groupOrders.Select(g => g.Id).ToList();
            var groups = await _context.Groups
                .Where(g => groupIds.Contains(g.Id) && g.UserId == userId)
                .ToListAsync();

            var groupDictionary = groups.ToDictionary(g => g.Id);

            // Обновляем порядок
            foreach (var item in groupOrders)
            {
                if (groupDictionary.TryGetValue(item.Id, out var group))
                {
                    group.Order = item.Order;
                }
                else
                {
                    _logger.LogWarning("Group {GroupId} not found for user {UserId}", item.Id, userId);
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Groups order successfully updated");
            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating groups order for user {UserId}", userId);
            return ServiceResult<bool>.Failure("Failed to update groups order");
        }
    }

    private async Task<Group?> GetUserGroupAsync(Guid groupId, Guid userId)
    {
        var group = await _context.Groups
            .FirstOrDefaultAsync(g => g.Id == groupId && g.UserId == userId);

        if (group == null)
        {
            _logger.LogWarning("Group {GroupId} not found for user {UserId}", groupId, userId);
        }

        return group;
    }

    private bool IsUniqueConstraintViolation(DbUpdateException exception)
    {
        return exception.InnerException is PostgresException { SqlState: "23505" };
    }
}