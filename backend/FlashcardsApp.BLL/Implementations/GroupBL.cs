using System.Text.RegularExpressions;
using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.BLL.Mapping;
using FlashcardsApp.DAL;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.DTOs.Groups.Requests;
using FlashcardsApp.Models.DTOs.Groups.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Group = FlashcardsApp.DAL.Models.Group;

namespace FlashcardsApp.BLL.Implementations;

public class GroupBL : IGroupBL
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GroupBL> _logger;

    public GroupBL(
        ApplicationDbContext context,
        ILogger<GroupBL> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResult<ResultGroupDto>> GetGroupByIdAsync(Guid groupId, Guid userId)
    {
        _logger.LogInformation("Fetching group {GroupId} for user {UserId}", groupId, userId);

        var group = await _context.Groups
            .AsNoTracking()
            .Where(g => g.Id == groupId && g.UserId == userId)  
            .Select(GroupMapper.ToDtoExpression())               
            .FirstOrDefaultAsync();

        if (group == null)
        {
            _logger.LogWarning("Group {GroupId} not found for user {UserId}", groupId, userId);
            return ServiceResult<ResultGroupDto>.Failure("Группа не найдена или доступ запрещен");
        }

        _logger.LogDebug("Group {GroupId} successfully retrieved", groupId);
        return ServiceResult<ResultGroupDto>.Success(group);
    }

    public async Task<ServiceResult<IEnumerable<ResultGroupDto>>> GetGroupsAsync(Guid userId)
    {
        _logger.LogInformation("Fetching all groups for user {UserId}", userId);

        var groups = await _context.Groups
            .Where(g => g.UserId == userId)
            .OrderBy(g => g.Order)
            .ThenBy(g => g.CreatedAt)
            .Select(GroupMapper.ToDtoExpression())
            .AsNoTracking()
            .ToListAsync();

        _logger.LogDebug("Retrieved {Count} groups for user {UserId}", groups.Count, userId);

        return ServiceResult<IEnumerable<ResultGroupDto>>.Success(groups);
    }

    public async Task<ServiceResult<ResultGroupDto>> CreateGroupAsync(CreateGroupDto model, Guid userId)
    {
        _logger.LogInformation("Creating new group '{GroupName}' for user {UserId}", model.Name, userId);

        var tags = await ProcessTagsAsync(model.Tags);

        var group = new Group
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            GroupName = model.Name,
            GroupIcon = model.GroupIcon,
            IsPublished = model.IsPublished,
            GroupColor = model.Color,
            CreatedAt = DateTime.UtcNow,
            Order = 0,
            Tags = tags 
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
            return ServiceResult<ResultGroupDto>.Failure("У вас уже есть группа с таким названием");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating group '{GroupName}' for user {UserId}", model.Name, userId);
            return ServiceResult<ResultGroupDto>.Failure("Ошибка при создании группы");
        }
    }

    public async Task<ServiceResult<ResultGroupDto>> UpdateGroupAsync(Guid groupId, Guid userId, CreateGroupDto model)
    {
        _logger.LogInformation("Updating group {GroupId} for user {UserId}", groupId, userId);

        var group = await GetUserGroupAsync(groupId, userId);

        if (group == null)
        {
            return ServiceResult<ResultGroupDto>.Failure("Группа не найдена или доступ запрещен");
        }

        group.GroupName = model.Name;
        group.GroupColor = model.Color;
        group.GroupIcon = model.GroupIcon;
        group.IsPublished = model.IsPublished;
        
        group.Tags.Clear();
        
        var newTags = await ProcessTagsAsync(model.Tags);
        
        foreach (var tag in newTags)
        {
            group.Tags.Add(tag);
        }

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Group {GroupId} successfully updated", groupId);
            return ServiceResult<ResultGroupDto>.Success(group.ToDto());
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            _logger.LogWarning("Duplicate group name '{GroupName}' for user {UserId}", model.Name, userId);
            return ServiceResult<ResultGroupDto>.Failure("У вас уже есть группа с таким названием");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating group {GroupId}", groupId);
            return ServiceResult<ResultGroupDto>.Failure("Ошибка при обновлении группы");
        }
    }

    public async Task<ServiceResult<ResultGroupDto>> UpdateAccessGroup(Guid groupId, Guid userId, bool isPublished)
    {
        var group = await GetUserGroupAsync(groupId, userId);

        if (group == null)
        {
            return ServiceResult<ResultGroupDto>.Failure("Группа не найдена или доступ запрещен");
        }

        group.IsPublished = isPublished;

        try
        {
            await _context.SaveChangesAsync();
            return ServiceResult<ResultGroupDto>.Success(group.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating group publishing {GroupId}", groupId);
            return ServiceResult<ResultGroupDto>.Failure("Ошибка при изменении статуса публикации");
        }
    }

    public async Task<ServiceResult<bool>> DeleteGroupAsync(Guid groupId, Guid userId)
    {
        _logger.LogInformation("Deleting group {GroupId} for user {UserId}", groupId, userId);

        var group = await GetUserGroupAsync(groupId, userId);

        if (group == null)
        {
            return ServiceResult<bool>.Failure("Группа не найдена или доступ запрещен");
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
            return ServiceResult<bool>.Failure("Ошибка при удалении группы");
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
                .ToDictionaryAsync(g => g.Id);

            foreach (var item in groupOrders)
            {
                if (groups.TryGetValue(item.Id, out var group))
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
            return ServiceResult<bool>.Failure("Ошибка при обновлении порядка групп");
        }
    }
    
    public async Task<ServiceResult<bool>> ChangeAccessGroupAsync(Guid groupId, Guid userId, bool isPublish)
    {
        var group = await GetUserGroupAsync(groupId, userId);

        if (group == null)
        {
            return ServiceResult<bool>.Failure("Группа не найдена");
        }

        if (isPublish)
        {
            var cardCount = await _context.Cards
                .CountAsync(c => c.GroupId == groupId);

            if (cardCount < 10)
            {
                return ServiceResult<bool>.Failure("В группе должно быть минимум 10 карточек для публикации");
            }
        }

        try
        {
            group.IsPublished = isPublish;

            if (!isPublish)
            {
                var subscriptions = await _context.UserGroupSubscriptions
                    .Where(s => s.GroupId == groupId)
                    .ToListAsync();

                if (subscriptions.Any())
                {
                    _context.UserGroupSubscriptions.RemoveRange(subscriptions);
                    _logger.LogInformation("Removed {Count} subscriptions for unpublished group {GroupId}", 
                        subscriptions.Count, groupId);
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Group {GroupId} access changed to {IsPublish}", groupId, isPublish);
            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing access for group {GroupId}", groupId);
            return ServiceResult<bool>.Failure("Ошибка при изменении доступа к группе");
        }
    }
    
    private async Task<Group?> GetUserGroupAsync(Guid groupId, Guid userId)
    {
        // Include(Tags) нужен, чтобы при обновлении группы мы могли корректно обновить список тегов
        return await _context.Groups
            .Include(g => g.Tags)
            .FirstOrDefaultAsync(g => g.Id == groupId && g.UserId == userId);
    }

    private bool IsUniqueConstraintViolation(DbUpdateException exception)
    {
        return exception.InnerException is PostgresException { SqlState: "23505" };
    }

    private async Task<List<Tag>> ProcessTagsAsync(List<string> tagNames)
    {
        var resultTags = new List<Tag>();
        if (tagNames == null || !tagNames.Any()) return resultTags;

        var uniqueNames = tagNames.Distinct().ToList();
        
        foreach (var tagName in uniqueNames)
        {
            if (string.IsNullOrWhiteSpace(tagName))
            {
                continue;
            }

            var slug = GetNewSlug(tagName);
            var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Slug == slug);

            if (existingTag != null)
            {
                resultTags.Add(existingTag);
            }
            else
            {
                var newTag = new Tag
                {
                    Id = Guid.NewGuid(),
                    Name = tagName.Trim(),
                    Slug = slug,
                    Color = GetRandomTagColor()
                };
                
                _context.Tags.Add(newTag);
                resultTags.Add(newTag);
            }
        }

        return resultTags;
    }

    private string GetNewSlug(string phrase)
    {
        string str = phrase.ToLower();
        str = Regex.Replace(str, @"\s+", "-");
        str = Regex.Replace(str, @"[^\w\-а-яё]", "");
        return str;
    }
    
    private string GetRandomTagColor()
    {
        var colors = new[] { "blue", "green", "red", "yellow", "purple", "pink", "indigo", "teal", "orange" };
        return colors[new Random().Next(colors.Length)];
    }
}