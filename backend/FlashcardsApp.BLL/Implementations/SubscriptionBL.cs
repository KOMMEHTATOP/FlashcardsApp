using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.DAL;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.DTOs;
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

    /// <summary>
    /// Получить детали публичной группы по ID, включая статус подписки текущего пользователя.
    /// </summary>
    public async Task<ServiceResult<PublicGroupDto>> GetPublicGroupDetailsAsync(Guid groupId, Guid currentUserId)
    {
        try
        {
            var groupDetails = await _context.Groups
                // Проверяем существование и статус публикации
                .Where(g => g.Id == groupId && g.IsPublished)
                .Select(g => new PublicGroupDto
                {
                    Id = g.Id,
                    GroupName = g.GroupName,
                    GroupColor = g.GroupColor,
                    GroupIcon = g.GroupIcon,
                    AuthorName = g.User.Login,
                    CardCount = g.Cards.Count,
                    SubscriberCount = g.SubscriberCount,
                    CreatedAt = g.CreatedAt,
                    // Проверяем статус подписки в одном запросе
                    IsSubscribed = g.Subscriptions.Any(s => s.SubscriberUserId == currentUserId)
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (groupDetails == null)
            {
                _logger.LogWarning("Публичная группа {GroupId} не найдена или не опубликована.", groupId);
                return ServiceResult<PublicGroupDto>.Failure("Группа не найдена или не опубликована");
            }

            return ServiceResult<PublicGroupDto>.Success(groupDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении деталей публичной группы {GroupId}", groupId);
            return ServiceResult<PublicGroupDto>.Failure("Ошибка при загрузке деталей группы");
        }
    }

    public async Task<ServiceResult<IEnumerable<PublicGroupDto>>> GetPublicGroupsAsync(
        Guid currentUserId,
        string? search = null,
        string sortBy = "date",
        int page = 1,
        int pageSize = 20,
        Guid? tagId = null)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var query = _context.Groups
                .Include(g => g.Tags)
                .Where(g => g.IsPublished && g.UserId != currentUserId)
                .AsNoTracking();

            // --- ФИЛЬТР ПО ТЕГУ ---
            if (tagId.HasValue)
            {
                query = query.Where(g => g.Tags.Any(t => t.Id == tagId));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(g =>
                    g.GroupName.ToLower().Contains(searchLower) ||
                    g.Tags.Any(t => t.Name.ToLower().Contains(searchLower))
                );
            }

            query = sortBy.ToLower() switch
            {
                "popular" => query.OrderByDescending(g => g.SubscriberCount)
                    .ThenByDescending(g => g.CreatedAt),
                "name" => query.OrderBy(g => g.GroupName)
                    .ThenByDescending(g => g.CreatedAt),
                _ => query.OrderByDescending(g => g.CreatedAt)
            };

            var groups = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(g => new PublicGroupDto
                {
                    Id = g.Id,
                    GroupName = g.GroupName,
                    GroupColor = g.GroupColor,
                    GroupIcon = g.GroupIcon,
                    AuthorName = g.User.Login,
                    CardCount = g.Cards.Count,
                    SubscriberCount = g.SubscriberCount,
                    CreatedAt = g.CreatedAt,
                    IsSubscribed = g.Subscriptions.Any(s => s.SubscriberUserId == currentUserId)
                })
                .ToListAsync();

            return ServiceResult<IEnumerable<PublicGroupDto>>.Success(groups);
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
                .OrderByDescending(s => s.SubscribedAt)
                .Select(s => new SubscribedGroupDto
                {
                    Id = s.Group.Id,
                    GroupName = s.Group.GroupName,
                    GroupColor = s.Group.GroupColor,
                    GroupIcon = s.Group.GroupIcon,
                    AuthorName = s.Group.User.Login,
                    CardCount = s.Group.Cards.Count,
                    SubscribedAt = s.SubscribedAt
                })
                .AsNoTracking()
                .ToListAsync();

            return ServiceResult<IEnumerable<SubscribedGroupDto>>.Success(subscriptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении подписок пользователя {UserId}", userId);
            return ServiceResult<IEnumerable<SubscribedGroupDto>>.Failure("Ошибка при загрузке подписок");
        }
    }

    public async Task<ServiceResult<bool>> SubscribeToGroupAsync(Guid groupId, Guid subscriberUserId)
    {
        try
        {
            var authorId = await _context.Groups
                .Where(g => g.Id == groupId && g.IsPublished)
                .Select(g => (Guid?)g.UserId)
                .FirstOrDefaultAsync();

            if (authorId == null)
            {
                return ServiceResult<bool>.Failure("Группа не найдена или не опубликована");
            }

            var alreadySubscribed = await _context.UserGroupSubscriptions
                .AnyAsync(s => s.GroupId == groupId && s.SubscriberUserId == subscriberUserId);

            if (alreadySubscribed)
            {
                return ServiceResult<bool>.Failure("Вы уже подписаны на эту группу");
            }

            var subscription = new UserGroupSubscription
            {
                SubscriberUserId = subscriberUserId, GroupId = groupId, SubscribedAt = DateTime.UtcNow
            };

            _context.UserGroupSubscriptions.Add(subscription);

            await _context.Groups
                .Where(g => g.Id == groupId)
                .ExecuteUpdateAsync(s => s.SetProperty(g => g.SubscriberCount, g => g.SubscriberCount + 1));

            await _context.Users
                .Where(u => u.Id == authorId.Value)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.TotalRating, u => u.TotalRating + 1));

            await _context.SaveChangesAsync();

            _logger.LogInformation("Пользователь {UserId} подписался на группу {GroupId}", subscriberUserId, groupId);
            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при подписке пользователя {UserId} на группу {GroupId}", subscriberUserId, groupId);
            return ServiceResult<bool>.Failure("Ошибка при подписке на группу");
        }
    }

    public async Task<ServiceResult<bool>> UnsubscribeFromGroupAsync(Guid groupId, Guid subscriberUserId)
    {
        try
        {
            // Получаем authorId сразу
            var authorId = await _context.UserGroupSubscriptions
                .Where(s => s.GroupId == groupId && s.SubscriberUserId == subscriberUserId)
                .Select(s => (Guid?)s.Group.UserId)
                .FirstOrDefaultAsync();

            if (authorId == null)
            {
                return ServiceResult<bool>.Failure("Подписка не найдена");
            }

            // Удаляем подписку
            var deleted = await _context.UserGroupSubscriptions
                .Where(s => s.GroupId == groupId && s.SubscriberUserId == subscriberUserId)
                .ExecuteDeleteAsync();

            if (deleted == 0)
            {
                return ServiceResult<bool>.Failure("Подписка не найдена");
            }

            // Обновляем счётчики (если упадёт — залогируется, но подписка уже удалена)
            await _context.Groups
                .Where(g => g.Id == groupId && g.SubscriberCount > 0)
                .ExecuteUpdateAsync(s => s.SetProperty(g => g.SubscriberCount, g => g.SubscriberCount - 1));

            await _context.Users
                .Where(u => u.Id == authorId.Value && u.TotalRating > 0)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.TotalRating, u => u.TotalRating - 1));

            _logger.LogInformation("Пользователь {UserId} отписался от группы {GroupId}", subscriberUserId, groupId);
            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отписке пользователя {UserId} от группы {GroupId}", subscriberUserId, groupId);
            return ServiceResult<bool>.Failure("Ошибка при отписке от группы");
        }
    }

    public async Task<ServiceResult<int>> GetAuthorRatingAsync(Guid authorUserId)
    {
        try
        {
            var rating = await _context.Users
                .Where(u => u.Id == authorUserId)
                .Select(u => u.TotalRating)
                .FirstOrDefaultAsync();

            if (rating == 0 && !await _context.Users.AnyAsync(u => u.Id == authorUserId))
            {
                return ServiceResult<int>.Failure("Автор не найден");
            }

            return ServiceResult<int>.Success(rating);
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

    public async Task<ServiceResult<IEnumerable<object>>> GetPublicGroupCardsAsync(Guid groupId)
    {
        try
        {
            // Проверяем, что группа существует и опубликована
            var isPublished = await _context.Groups
                .Where(g => g.Id == groupId)
                .Select(g => g.IsPublished)
                .FirstOrDefaultAsync();

            if (!isPublished)
            {
                return ServiceResult<IEnumerable<object>>.Failure("Группа не найдена или не опубликована");
            }

            // Получаем карточки группы
            var cards = await _context.Cards
                .Where(c => c.GroupId == groupId)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new
                {
                    c.CardId, c.Question, c.Answer, c.CreatedAt
                })
                .AsNoTracking()
                .ToListAsync();

            _logger.LogInformation("Получено {Count} карточек для публичной группы {GroupId}", cards.Count, groupId);

            return ServiceResult<IEnumerable<object>>.Success(cards);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении карточек публичной группы {GroupId}", groupId);
            return ServiceResult<IEnumerable<object>>.Failure("Ошибка при загрузке карточек");
        }
    }

    public async Task<ServiceResult<IEnumerable<TagDto>>> GetTagsAsync()
    {
        try
        {
            // Берем только те теги, которые используются в ПУБЛИЧНЫХ группах (чтобы не показывать пустые)
            var tags = await _context.Tags
                .Where(t => t.Groups.Any(g => g.IsPublished))
                .Select(t => new TagDto
                {
                    Id = t.Id, Name = t.Name, Color = t.Color
                })
                .Distinct()
                .OrderBy(t => t.Name)
                .AsNoTracking()
                .ToListAsync();

            return ServiceResult<IEnumerable<TagDto>>.Success(tags);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка загрузки тегов");
            return ServiceResult<IEnumerable<TagDto>>.Failure("Ошибка загрузки тегов");
        }
    }
}
