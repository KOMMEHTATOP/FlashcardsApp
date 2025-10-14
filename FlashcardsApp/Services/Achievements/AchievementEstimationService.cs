using FlashcardsApp.Interfaces.Achievements;
using FlashcardsApp.Models;
using FlashcardsAppContracts.Enums;

namespace FlashcardsApp.Services.Achievements;

/// <summary>
/// Сервис для оценки времени до получения достижения
/// Отвечает ТОЛЬКО за расчет оценки дней на основе статистики пользователя
/// </summary>
public class AchievementEstimationService : IAchievementEstimationService
{
    // Константы для оценки среднего темпа (можно вынести в конфигурацию)
    private const int DefaultCardsPerDay = 5;
    private const int DefaultCreatedCardsPerDay = 2;
    private const int DefaultXpPerDay = 100;
    private const int DefaultStudyHoursPerDay = 1;

    /// <summary>
    /// Оценить количество дней до выполнения достижения
    /// </summary>
    public int EstimateDaysToComplete(
        AchievementConditionType conditionType, 
        int remainingValue, 
        UserStatistics userStats)
    {
        // Защита от некорректных данных
        if (remainingValue <= 0)
            return 0;

        return conditionType switch
        {
            // Для streak-достижений оценка точная
            AchievementConditionType.CurrentStreak => remainingValue,
            AchievementConditionType.BestStreak => remainingValue,

            // Для изучения карточек - анализируем темп пользователя
            AchievementConditionType.TotalCardsStudied => 
                EstimateByUserPace(
                    remainingValue, 
                    userStats.TotalCardsStudied, 
                    userStats.LastStudyDate, 
                    DefaultCardsPerDay),

            // Для создания карточек
            AchievementConditionType.TotalCardsCreated => 
                EstimateByUserPace(
                    remainingValue, 
                    userStats.TotalCardsCreated, 
                    userStats.LastStudyDate, 
                    DefaultCreatedCardsPerDay),

            // Для уровней - оцениваем через XP
            AchievementConditionType.Level => 
                EstimateDaysForLevel(remainingValue, userStats),

            // Для XP
            AchievementConditionType.TotalXP => 
                EstimateByUserPace(
                    remainingValue, 
                    userStats.TotalXP, 
                    userStats.LastStudyDate, 
                    DefaultXpPerDay),

            // Для времени обучения
            AchievementConditionType.TotalStudyTimeHours => 
                EstimateByUserPace(
                    remainingValue, 
                    (int)userStats.TotalStudyTime.TotalHours, 
                    userStats.LastStudyDate, 
                    DefaultStudyHoursPerDay),

            // Для PerfectRatingsStreak невозможно точно предсказать
            AchievementConditionType.PerfectRatingsStreak => 0,

            // По умолчанию
            _ => 0
        };
    }

    #region Private Helper Methods

    /// <summary>
    /// Оценка дней на основе темпа пользователя
    /// Если пользователь неактивен или данных мало - используем дефолтный темп
    /// </summary>
    private static int EstimateByUserPace(
        int remainingValue, 
        int currentTotal, 
        DateTime? lastActivity, 
        int defaultPacePerDay)
    {
        // Если пользователь новый или неактивен - используем дефолтный темп
        if (currentTotal < 5 || lastActivity == null)
        {
            return (int)Math.Ceiling((double)remainingValue / defaultPacePerDay);
        }

        // Проверяем активность - если не занимался больше 30 дней, используем дефолт
        var daysSinceLastActivity = (DateTime.UtcNow - lastActivity.Value).Days;
        if (daysSinceLastActivity > 30)
        {
            return (int)Math.Ceiling((double)remainingValue / defaultPacePerDay);
        }

        // Вычисляем реальный темп пользователя
        // УПРОЩЕННАЯ ОЦЕНКА: currentTotal / daysSinceLastActivity
        // В реальности лучше брать последние 30 дней из StudyHistory
        var estimatedDaysActive = Math.Max(1, daysSinceLastActivity);
        var userPacePerDay = (double)currentTotal / estimatedDaysActive;

        // Защита от слишком оптимистичных оценок
        // Если темп слишком высокий - ограничиваем его (возможно аномалия)
        userPacePerDay = Math.Min(userPacePerDay, defaultPacePerDay * 3);

        // Защита от деления на ноль
        if (userPacePerDay < 0.1)
        {
            return (int)Math.Ceiling((double)remainingValue / defaultPacePerDay);
        }

        return (int)Math.Ceiling(remainingValue / userPacePerDay);
    }

    /// <summary>
    /// Оценка дней для достижения уровня
    /// Учитываем экспоненциальный рост XP для уровней
    /// </summary>
    private static int EstimateDaysForLevel(int targetLevel, UserStatistics userStats)
    {
        // Текущий уровень пользователя
        var currentLevel = userStats.Level;
        var currentXP = userStats.TotalXP;

        // Вычисляем сколько XP нужно для целевого уровня
        // Формула: XP_для_уровня = 100 * уровень^2
        // (Эта формула должна совпадать с GamificationService)
        var requiredXP = CalculateXPForLevel(targetLevel);
        var remainingXP = requiredXP - currentXP;

        // Если уже достигнут или превышен
        if (remainingXP <= 0)
            return 0;

        // Оцениваем темп набора XP
        return EstimateByUserPace(
            remainingXP, 
            currentXP, 
            userStats.LastStudyDate, 
            DefaultXpPerDay);
    }

    /// <summary>
    /// Вычислить требуемый XP для уровня
    /// ВАЖНО: Эта формула должна совпадать с GamificationService.CalculateLevelFromXP()
    /// </summary>
    private static int CalculateXPForLevel(int level)
    {
        // Формула: XP = 100 * level^2
        // Уровень 1 = 100 XP
        // Уровень 5 = 2500 XP
        // Уровень 10 = 10000 XP
        return 100 * level * level;
    }

    #endregion
}