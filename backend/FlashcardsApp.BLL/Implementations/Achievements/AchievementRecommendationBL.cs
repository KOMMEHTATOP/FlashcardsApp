using FlashcardsApp.BLL.Interfaces.Achievements;
using FlashcardsApp.DAL;
using FlashcardsApp.Models.DTOs.Achievements.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace FlashcardsApp.BLL.Implementations.Achievements;

/// <summary>
/// Сервис для генерации рекомендаций достижений
/// Отвечает ТОЛЬКО за создание списка рекомендаций на основе прогресса
/// Оркестрирует работу других сервисов: Progress + Motivation + Estimation
/// </summary>
public class AchievementRecommendationBL : IAchievementRecommendationBL
{
    private readonly IAchievementProgressBL _progressBl;
    private readonly IAchievementMotivationBL _motivationBl;
    private readonly IAchievementEstimationBL _estimationBl;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AchievementRecommendationBL> _logger;

    public AchievementRecommendationBL(
        IAchievementProgressBL progressBl,
        IAchievementMotivationBL motivationBl,
        IAchievementEstimationBL estimationBl,
        ApplicationDbContext context,
        ILogger<AchievementRecommendationBL> logger)
    {
        _progressBl = progressBl;
        _motivationBl = motivationBl;
        _estimationBl = estimationBl;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Получить рекомендации: какие достижения пользователь скоро получит
    /// </summary>
    public async Task<ServiceResult<IEnumerable<AchievementRecommendationDto>>> GetAchievementRecommendationsAsync(
        Guid userId, 
        int count = 3)
    {
        try
        {
            if (count is <= 0 or > 10)
            {
                return ServiceResult<IEnumerable<AchievementRecommendationDto>>.Failure(
                    "Количество рекомендаций должно быть от 1 до 10");
            }

            // 1. Получаем прогресс всех достижений
            var allProgressResult = await _progressBl.GetAllAchievementsProgressAsync(userId);
            
            if (!allProgressResult.IsSuccess || allProgressResult.Data == null)
            {
                return ServiceResult<IEnumerable<AchievementRecommendationDto>>.Failure(
                    allProgressResult.Errors.FirstOrDefault() ?? "Не удалось получить прогресс достижений");
            }

            // 2. Получаем статистику пользователя для оценки времени
            var userStats = await _context.UserStatistics
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (userStats == null)
            {
                return ServiceResult<IEnumerable<AchievementRecommendationDto>>.Failure(
                    "Статистика пользователя не найдена");
            }

            // 3. Фильтруем и сортируем достижения
            var recommendations = allProgressResult.Data
                .Where(p => p is { IsUnlocked: false, ProgressPercentage: > 0 }) // Только неразблокированные с прогрессом
                .OrderByDescending(p => p.ProgressPercentage)          // Сначала ближайшие к завершению
                .ThenBy(p => p.Rarity)                                 // При равном прогрессе - более простые
                .Take(count)                                              // Берем топ-N
                .Select(p =>
                {
                    var remainingValue = p.ConditionValue - p.CurrentValue;
                    
                    // 4. Генерируем мотивационное сообщение
                    var motivationalMessage = _motivationBl.GenerateMotivationalMessage(
                        p.ConditionType,
                        remainingValue,
                        p.Name);
                    
                    // 5. Оцениваем время до получения
                    var estimatedDays = _estimationBl.EstimateDaysToComplete(
                        p.ConditionType,
                        remainingValue,
                        userStats);

                    return new AchievementRecommendationDto
                    {
                        AchievementId = p.AchievementId,
                        Name = p.Name,
                        Description = p.Description,
                        IconUrl = p.IconUrl,
                        Gradient = p.Gradient,
                        Rarity = p.Rarity,
                        ProgressPercentage = p.ProgressPercentage,
                        RemainingValue = remainingValue,
                        ConditionType = p.ConditionType,
                        MotivationalMessage = motivationalMessage,
                        EstimatedDaysToComplete = estimatedDays
                    };
                })
                .ToList();

            _logger.LogInformation(
                "Сгенерировано {Count} рекомендаций для пользователя {UserId}", 
                recommendations.Count, 
                userId);

            return ServiceResult<IEnumerable<AchievementRecommendationDto>>.Success(recommendations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Ошибка при получении рекомендаций достижений для пользователя {UserId}", 
                userId);
            return ServiceResult<IEnumerable<AchievementRecommendationDto>>.Failure(
                "Ошибка при получении рекомендаций достижений");
        }
    }
}