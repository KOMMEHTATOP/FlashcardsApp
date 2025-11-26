using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.DAL;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.DTOs.Leaderboard;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FlashcardsApp.BLL.Implementations;

public class LeaderboardBL : ILeaderboardBL
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<LeaderboardBL> _logger;

    public LeaderboardBL(ApplicationDbContext dbContext, ILogger<LeaderboardBL> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<ServiceResult<LeaderboardDto>> GetLeaderboardAsync(Guid userId)
    {
        try
        {
            var top10Users = await _dbContext.Users
                .OrderByDescending(u => u.TotalRating)
                .ThenByDescending(u => u.RatingLastUpdatedAt)
                .Take(10)
                .Select(u => new LeaderboardEntryDto
                {
                    UserId = u.Id, 
                    Login = u.Login, 
                    TotalRating = u.TotalRating, 
                    Position = 0
                })
                .AsNoTracking()
                .ToListAsync();

            for (var i = 0; i < top10Users.Count; i++)
            {
                top10Users[i].Position = i + 1;
            }

            var isInTop10 = top10Users.Any(u => u.UserId == userId);

            if (!isInTop10)
            {
                var currentUser = await _dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (currentUser == null)
                {
                    return ServiceResult<LeaderboardDto>.Failure("Пользователь не найден");
                }

                var betterUsersCount = await _dbContext.Users
                    .Where(u =>
                        u.TotalRating > currentUser.TotalRating ||
                        (u.TotalRating == currentUser.TotalRating &&
                         u.RatingLastUpdatedAt > currentUser.RatingLastUpdatedAt))
                    .CountAsync();

                var currentUserPosition = betterUsersCount + 1;

                top10Users.Add(new LeaderboardEntryDto
                {
                    Position = currentUserPosition,
                    UserId = currentUser.Id,
                    Login = currentUser.Login,
                    TotalRating = currentUser.TotalRating
                });
            }

            var totalCount = await _dbContext.Users
                .CountAsync(u => u.TotalRating > 0);

            return ServiceResult<LeaderboardDto>.Success(new LeaderboardDto
            {
                TopList = top10Users, TotalUsersCount = totalCount
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка при получении лидерборда для пользователя {UserId}", userId);
            return ServiceResult<LeaderboardDto>.Failure("Ошибка при загрузке лидерборда");
        }
    }
}
