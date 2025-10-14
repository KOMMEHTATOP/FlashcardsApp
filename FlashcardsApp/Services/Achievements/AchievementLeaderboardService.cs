using FlashcardsApp.Data;
using FlashcardsApp.Interfaces.Achievements;
using FlashcardsAppContracts.DTOs.Achievements.Responses;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.Services.Achievements;

/// <summary>
/// Сервис для работы с таблицей лидеров по достижениям
/// </summary>
public class AchievementLeaderboardService : IAchievementLeaderboardService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AchievementLeaderboardService> _logger;

    public AchievementLeaderboardService(
        ApplicationDbContext context,
        ILogger<AchievementLeaderboardService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<LeaderboardEntryDto>>> GetTopUsersByAchievementsAsync(int count = 10)
    {
        _logger.LogInformation("Fetching top {Count} users by achievements", count);

        if (count < 1 || count > 100)
        {
            _logger.LogWarning("Invalid count parameter: {Count}", count);
            return ServiceResult<IEnumerable<LeaderboardEntryDto>>.Failure("Count must be between 1 and 100");
        }

        try
        {
            var leaderboard = await _context.Users
                .Select(u => new
                {
                    User = u,
                    AchievementCount = u.UserAchievements.Count,
                    TotalXP = u.Statistics != null ? u.Statistics.TotalXP : 0
                })
                .OrderByDescending(x => x.AchievementCount)
                .ThenByDescending(x => x.TotalXP)
                .Take(count)
                .Select((x, index) => new LeaderboardEntryDto
                {
                    UserId = x.User.Id,
                    Username = x.User.UserName ?? "Unknown",
                    AvatarUrl = null, // TODO: Добавить, когда будет поле Avatar в User
                    AchievementCount = x.AchievementCount,
                    TotalXP = x.TotalXP,
                    Position = index + 1,
                    IsCurrentUser = false
                })
                .ToListAsync();

            _logger.LogDebug("Retrieved {Count} users for leaderboard", leaderboard.Count);
            return ServiceResult<IEnumerable<LeaderboardEntryDto>>.Success(leaderboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching leaderboard");
            return ServiceResult<IEnumerable<LeaderboardEntryDto>>.Failure("Failed to fetch leaderboard");
        }
    }

    public async Task<ServiceResult<int>> GetUserLeaderboardPositionAsync(Guid userId)
    {
        _logger.LogInformation("Fetching leaderboard position for user {UserId}", userId);

        try
        {
            // Получаем количество достижений пользователя
            var userAchievementCount = await _context.UserAchievements
                .CountAsync(ua => ua.UserId == userId);

            // Считаем, сколько пользователей имеют больше достижений
            var usersAbove = await _context.Users
                .Select(u => new
                {
                    UserId = u.Id,
                    AchievementCount = u.UserAchievements.Count
                })
                .CountAsync(x => x.AchievementCount > userAchievementCount);

            var position = usersAbove + 1;

            _logger.LogDebug("User {UserId} is at position {Position}", userId, position);
            return ServiceResult<int>.Success(position);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user position for user {UserId}", userId);
            return ServiceResult<int>.Failure("Failed to fetch user position");
        }
    }

    public async Task<ServiceResult<IEnumerable<LeaderboardEntryDto>>> GetLeaderboardWithUserAsync(Guid userId, int topCount = 10)
    {
        _logger.LogInformation("Fetching leaderboard with user {UserId} position", userId);

        if (topCount < 1 || topCount > 100)
        {
            _logger.LogWarning("Invalid topCount parameter: {TopCount}", topCount);
            return ServiceResult<IEnumerable<LeaderboardEntryDto>>.Failure("Count must be between 1 and 100");
        }

        try
        {
            // Получаем топ пользователей
            var topUsers = await _context.Users
                .Select(u => new
                {
                    User = u,
                    AchievementCount = u.UserAchievements.Count,
                    TotalXP = u.Statistics != null ? u.Statistics.TotalXP : 0
                })
                .OrderByDescending(x => x.AchievementCount)
                .ThenByDescending(x => x.TotalXP)
                .Take(topCount)
                .ToListAsync();

            // Проверяем, есть ли текущий пользователь в топе
            var currentUserInTop = topUsers.Any(x => x.User.Id == userId);

            var leaderboard = topUsers
                .Select((x, index) => new LeaderboardEntryDto
                {
                    UserId = x.User.Id,
                    Username = x.User.UserName ?? "Unknown",
                    AvatarUrl = null,
                    AchievementCount = x.AchievementCount,
                    TotalXP = x.TotalXP,
                    Position = index + 1,
                    IsCurrentUser = x.User.Id == userId
                })
                .ToList();

            // Если пользователя нет в топе, добавляем его отдельно
            if (!currentUserInTop)
            {
                var currentUser = await _context.Users
                    .Where(u => u.Id == userId)
                    .Select(u => new
                    {
                        User = u,
                        AchievementCount = u.UserAchievements.Count,
                        TotalXP = u.Statistics != null ? u.Statistics.TotalXP : 0
                    })
                    .FirstOrDefaultAsync();

                if (currentUser != null)
                {
                    // Вычисляем позицию пользователя
                    var usersAbove = await _context.Users
                        .Select(u => new
                        {
                            AchievementCount = u.UserAchievements.Count
                        })
                        .CountAsync(x => x.AchievementCount > currentUser.AchievementCount);

                    var userPosition = usersAbove + 1;

                    leaderboard.Add(new LeaderboardEntryDto
                    {
                        UserId = currentUser.User.Id,
                        Username = currentUser.User.UserName ?? "Unknown",
                        AvatarUrl = null,
                        AchievementCount = currentUser.AchievementCount,
                        TotalXP = currentUser.TotalXP,
                        Position = userPosition,
                        IsCurrentUser = true
                    });

                    _logger.LogDebug("Added user {UserId} at position {Position} to leaderboard", userId, userPosition);
                }
            }

            _logger.LogDebug("Retrieved leaderboard with {Count} entries", leaderboard.Count);
            return ServiceResult<IEnumerable<LeaderboardEntryDto>>.Success(leaderboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching leaderboard with user {UserId}", userId);
            return ServiceResult<IEnumerable<LeaderboardEntryDto>>.Failure("Failed to fetch leaderboard");
        }
    }
}