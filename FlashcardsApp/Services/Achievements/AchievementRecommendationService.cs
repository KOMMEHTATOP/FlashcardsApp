using FlashcardsApp.Data;
using FlashcardsApp.Interfaces.Achievements;
using FlashcardsAppContracts.DTOs.Achievements.Responses;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.Services.Achievements;

/// <summary>
/// Сервис для генерации рекомендаций достижений
/// Отвечает ТОЛЬКО за создание списка рекомендаций на основе прогресса
/// Оркестрирует работу других сервисов: Progress + Motivation + Estimation
/// </summary>
public class AchievementRecommendationService : IAchievementRecommendationService
{
    private readonly IAchievementProgressService _progressService;
    private readonly IAchievementMotivationService _motivationService;
    private readonly IAchievementEstimationService _estimationService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AchievementRecommendationService> _logger;

    public AchievementRecommendationService(
        IAchievementProgressService progressService,
        IAchievementMotivationService motivationService,
        IAchievementEstimationService estimationService,
        ApplicationDbContext context,
        ILogger<AchievementRecommendationService> logger)
    {
        _progressService = progressService;
        _motivationService = motivationService;
        _estimationService = estimationService;
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
            // Валидация входных данных
            if (count <= 0 || count > 10)
            {
                return ServiceResult<IEnumerable<AchievementRecommendationDto>>.Failure(
                    "Количество рекомендаций должно быть от 1 до 10");
            }

            // 1. Получаем прогресс всех достижений
            var allProgressResult = await _progressService.GetAllAchievementsProgressAsync(userId);
            
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
                .Where(p => !p.IsUnlocked && p.ProgressPercentage > 0) // Только неразблокированные с прогрессом
                .OrderByDescending(p => p.ProgressPercentage)          // Сначала ближайшие к завершению
                .ThenBy(p => p.Rarity)                                 // При равном прогрессе - более простые
                .Take(count)                                           // Берем топ-N
                .Select(p =>
                {
                    var remainingValue = p.ConditionValue - p.CurrentValue;
                    
                    // 4. Генерируем мотивационное сообщение
                    var motivationalMessage = _motivationService.GenerateMotivationalMessage(
                        p.ConditionType,
                        remainingValue,
                        p.Name);
                    
                    // 5. Оцениваем время до получения
                    var estimatedDays = _estimationService.EstimateDaysToComplete(
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