using FlashcardsApp.Configuration; 
using FlashcardsApp.Data;
using FlashcardsApp.Interfaces;
using FlashcardsApp.Interfaces.Achievements;
using FlashcardsAppContracts.DTOs.Achievements.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FlashcardsApp.Services.Achievements;

/// <summary>
/// Сервис для начисления наград за достижения
/// </summary>
public class AchievementRewardService : IAchievementRewardService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AchievementRewardService> _logger;
    private readonly IGamificationService _gamificationService;
    private readonly RewardSettings _settings;

    public AchievementRewardService(
        ApplicationDbContext context,
        ILogger<AchievementRewardService> logger,
        IGamificationService gamificationService,
        IOptions<RewardSettings> settingsOptions)
    {
        _context = context;
        _logger = logger;
        _gamificationService = gamificationService;
        _settings = settingsOptions.Value;
    }

    /// <summary>
    /// Начислить награду за разблокированное достижение
    /// </summary>
    public async Task<ServiceResult<AchievementRewardDto>> AwardBonusForAchievementAsync(Guid userId, Guid achievementId)
    {
        try
        {
            // Получаем информацию о достижении
            var achievement = await _context.Achievements
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == achievementId);

            if (achievement == null)
            {
                return ServiceResult<AchievementRewardDto>.Failure("Achievement not found");
            }

            // Базовая награда из конфига
            var baseXP = _settings.Base.XPPerAchievement;
            var coins = _settings.Base.CoinsPerAchievement;

            // Начисляем XP через GamificationService
            var addResult = await _gamificationService.AddXPToUserAsync(userId, baseXP);
            
            if (!addResult.IsSuccess)
            {
                _logger.LogWarning("Failed to add XP for achievement {AchievementId} to user {UserId}", 
                    achievementId, userId);
                return ServiceResult<AchievementRewardDto>.Failure("Failed to award XP");
            }

            // TODO: Начислить монеты когда появится функционал
            // await _coinService.AddCoinsAsync(userId, coins);

            var rewardDto = new AchievementRewardDto
            {
                AchievementId = achievementId,
                AchievementName = achievement.Name,
                XPAwarded = baseXP,
                CoinsAwarded = coins,
                AwardedAt = DateTime.UtcNow
            };

            _logger.LogInformation(
                "Awarded {XP} XP and {Coins} coins to user {UserId} for achievement '{Achievement}'",
                baseXP, coins, userId, achievement.Name);

            // TODO: Сохранить в таблицу UserRewards для истории
            
            return ServiceResult<AchievementRewardDto>.Success(rewardDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Failed to award bonus for achievement {AchievementId} to user {UserId}", 
                achievementId, userId);
            return ServiceResult<AchievementRewardDto>.Failure("Failed to award bonus");
        }
    }

    /// <summary>
    /// Получить историю наград пользователя
    /// TODO: Реализовать когда появится таблица UserRewards
    /// </summary>
    public Task<ServiceResult<IEnumerable<AchievementRewardDto>>> GetUserRewardHistoryAsync(Guid userId)
    {
        _logger.LogDebug(
            "GetUserRewardHistoryAsync called for user {UserId} — history storage not implemented yet", 
            userId);

        // TODO: создать сущность UserReward и DbSet<UserReward> в ApplicationDbContext
        return Task.FromResult(
            ServiceResult<IEnumerable<AchievementRewardDto>>.Success(
                Enumerable.Empty<AchievementRewardDto>()));
    }
}