// FlashcardsApp.BLL.Implementations/SubscriptionBL.cs

using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.DAL;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.DTOs.Subscriptions.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FlashcardsApp.BLL.Implementations;

public class SubscriptionBL : ISubscriptionBL
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SubscriptionBL> _logger;

    public SubscriptionBL(ApplicationDbContext context, ILogger<SubscriptionBL> logger)
    {
        _context = context;
        _logger = logger;
    }

// SubscriptionBL.cs
    public async Task<ServiceResult<IEnumerable<PublicGroupDto>>> GetPublicGroupsAsync(string? search = null)
    {
        try
        {
            var query = _context.Groups
                .Where(g => g.IsPublished)
                .Include(g => g.User) // User всегда есть
                .Include(g => g.Cards)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(g => g.GroupName.Contains(search));
            }

            var groups = await query
                .OrderByDescending(g => g.SubscriberCount)
                .ThenBy(g => g.GroupName)
                .ToListAsync();

            var result = groups.Select(g => new PublicGroupDto
            {
                Id = g.Id,
                GroupName = g.GroupName,
                GroupColor = g.GroupColor,
                GroupIcon = g.GroupIcon,
                AuthorName = g.User.Login, 
                CardCount = g.Cards?.Count ?? 0,
                SubscriberCount = g.SubscriberCount,
                CreatedAt = g.CreatedAt
            });

            return ServiceResult<IEnumerable<PublicGroupDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении публичных групп");
            return ServiceResult<IEnumerable<PublicGroupDto>>.Failure("Ошибка при загрузке групп");
        }
    }

    public async Task<ServiceResult<IEnumerable<SubscribedGroupDto>>> GetSubscribedGroupsAsync(Guid userId)
    {
        try
        {
            var subscriptions = await _context.UserGroupSubscriptions
                .Where(s => s.SubscriberUserId == userId)
                .Include(s => s.Group)
                .ThenInclude(g => g.User)
                .Include(s => s.Group)
                .ThenInclude(g => g.Cards)
                .OrderByDescending(s => s.SubscribedAt)
                .ToListAsync();

            var result = subscriptions.Select(s => new SubscribedGroupDto
            {
                Id = s.Group.Id,
                GroupName = s.Group.GroupName,
                GroupColor = s.Group.GroupColor,
                GroupIcon = s.Group.GroupIcon,
                AuthorName = s.Group.User.Login, 
                CardCount = s.Group.Cards?.Count ?? 0,
                SubscribedAt = s.SubscribedAt
            });

            return ServiceResult<IEnumerable<SubscribedGroupDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении подписок пользователя {UserId}", userId);
            return ServiceResult<IEnumerable<SubscribedGroupDto>>.Failure("Ошибка при загрузке подписок");
        }
    }

    public async Task<ServiceResult<bool>> SubscribeToGroupAsync(Guid groupId, Guid subscriberUserId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Проверяем существование группы
            var group = await _context.Groups
                .Include(g => g.User)
                .FirstOrDefaultAsync(g => g.Id == groupId && g.IsPublished);

            if (group == null)
            {
                return ServiceResult<bool>.Failure("Группа не найдена или не опубликована");
            }

            // Проверяем нет ли уже подписки
            var existingSubscription = await _context.UserGroupSubscriptions
                .FirstOrDefaultAsync(s => s.GroupId == groupId && s.SubscriberUserId == subscriberUserId);

            if (existingSubscription != null)
            {
                return ServiceResult<bool>.Failure("Вы уже подписаны на эту группу");
            }

            // Создаем подписку
            var subscription = new UserGroupSubscription
            {
                SubscriberUserId = subscriberUserId, GroupId = groupId, SubscribedAt = DateTime.UtcNow
            };

            _context.UserGroupSubscriptions.Add(subscription);

            // Обновляем счетчики
            group.SubscriberCount++;
            group.User.TotalRating++;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Пользователь {UserId} подписался на группу {GroupId}", subscriberUserId, groupId);
            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Ошибка при подписке пользователя {UserId} на группу {GroupId}", subscriberUserId, groupId);
            return ServiceResult<bool>.Failure("Ошибка при подписке на группу");
        }
    }

    public async Task<ServiceResult<bool>> UnsubscribeFromGroupAsync(Guid groupId, Guid subscriberUserId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var subscription = await _context.UserGroupSubscriptions
                .Include(s => s.Group)
                .ThenInclude(g => g.User)
                .FirstOrDefaultAsync(s => s.GroupId == groupId && s.SubscriberUserId == subscriberUserId);

            if (subscription == null)
            {
                return ServiceResult<bool>.Failure("Подписка не найдена");
            }

            _context.UserGroupSubscriptions.Remove(subscription);

            // Обновляем счетчики
            subscription.Group.SubscriberCount = Math.Max(0, subscription.Group.SubscriberCount - 1);
            subscription.Group.User.TotalRating = Math.Max(0, subscription.Group.User.TotalRating - 1);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Пользователь {UserId} отписался от группы {GroupId}", subscriberUserId, groupId);
            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Ошибка при отписке пользователя {UserId} от группы {GroupId}", subscriberUserId, groupId);
            return ServiceResult<bool>.Failure("Ошибка при отписке от группы");
        }
    }

    public async Task<ServiceResult<int>> GetAuthorRatingAsync(Guid authorUserId)
    {
        try
        {
            var author = await _context.Users.FindAsync(authorUserId);
            return ServiceResult<int>.Success(author?.TotalRating ?? 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении рейтинга автора {AuthorId}", authorUserId);
            return ServiceResult<int>.Failure("Ошибка при получении рейтинга");
        }
    }

    public async Task<ServiceResult<bool>> IsSubscribedAsync(Guid groupId, Guid userId)
    {
        try
        {
            var isSubscribed = await _context.UserGroupSubscriptions
                .AnyAsync(s => s.GroupId == groupId && s.SubscriberUserId == userId);

            return ServiceResult<bool>.Success(isSubscribed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при проверке подписки пользователя {UserId} на группу {GroupId}", userId, groupId);
            return ServiceResult<bool>.Failure("Ошибка при проверке подписки");
        }
    }
}
