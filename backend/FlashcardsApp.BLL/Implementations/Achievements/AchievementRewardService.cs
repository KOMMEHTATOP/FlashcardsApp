using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.BLL.Interfaces.Achievements;
using FlashcardsApp.DAL;
using FlashcardsApp.Models.Constants;
using FlashcardsApp.Models.DTOs.Achievements.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;


namespace FlashcardsApp.BLL.Implementations.Achievements;

/// <summary>
/// Сервис для начисления наград за достижения
/// </summary>
public class AchievementRewardService : IAchievementRewardService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AchievementRewardService> _logger;
    private readonly IGamificationBL _gamificationBl;
    private readonly RewardSettings _settings;

    public AchievementRewardService(
        ApplicationDbContext context,
        ILogger<AchievementRewardService> logger,
        IGamificationBL gamificationBl,
        IOptions<RewardSettings> settingsOptions)
    {
        _context = context;
        _logger = logger;
        _gamificationBl = gamificationBl;
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
            var addResult = await _gamificationBl.AddXPToUserAsync(userId, baseXP);

            if (!addResult.IsSuccess)
            {
                _logger.LogWarning("Failed to add XP for achievement {AchievementId} to user {UserId}",
                    achievementId, userId);
                return ServiceResult<AchievementRewardDto>.Failure("Failed to award XP");
            }

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
    
}