using FlashcardsApp.Interfaces.Achievements;
using FlashcardsAppContracts.Enums;

namespace FlashcardsApp.Services.Achievements;

/// <summary>
/// Сервис для генерации мотивационных сообщений
/// Отвечает ТОЛЬКО за текстовую мотивацию и локализацию (склонения слов)
/// </summary>
public class AchievementMotivationService : IAchievementMotivationService
{
    /// <summary>
    /// Сгенерировать мотивационное сообщение для достижения
    /// </summary>
    public string GenerateMotivationalMessage(
        AchievementConditionType conditionType, 
        int remainingValue, 
        string achievementName)
    {
        return conditionType switch
        {
            AchievementConditionType.TotalCardsStudied => 
                $"Изучите еще {remainingValue} {GetCardWord(remainingValue)} для получения «{achievementName}»!",
            
            AchievementConditionType.TotalCardsCreated => 
                $"Создайте еще {remainingValue} {GetCardWord(remainingValue)} для получения «{achievementName}»!",
            
            AchievementConditionType.CurrentStreak => 
                $"Занимайтесь еще {remainingValue} {GetDayWord(remainingValue)} подряд для получения «{achievementName}»!",
            
            AchievementConditionType.BestStreak => 
                $"Побейте свой рекорд на {remainingValue} {GetDayWord(remainingValue)} для получения «{achievementName}»!",
            
            AchievementConditionType.Level => 
                $"Достигните {remainingValue}-го уровня для получения «{achievementName}»!",
            
            AchievementConditionType.TotalXP => 
                $"Заработайте еще {remainingValue} XP для получения «{achievementName}»!",
            
            AchievementConditionType.PerfectRatingsStreak => 
                $"Получите еще {remainingValue} {GetPerfectRatingWord(remainingValue)} подряд для получения «{achievementName}»!",
            
            AchievementConditionType.TotalStudyTimeHours => 
                $"Занимайтесь еще {remainingValue} {GetHourWord(remainingValue)} для получения «{achievementName}»!",
            
            _ => $"Выполните условие для получения «{achievementName}»!"
        };
    }

    #region Private Helper Methods - Склонения русских слов

    /// <summary>
    /// Склонение слова "карточка"
    /// Примеры: 1 карточку, 2 карточки, 5 карточек, 21 карточку
    /// </summary>
    private static string GetCardWord(int count)
    {
        var lastDigit = count % 10;
        var lastTwoDigits = count % 100;

        // Исключение для чисел 11-14 (они всегда "карточек")
        if (lastTwoDigits >= 11 && lastTwoDigits <= 14)
            return "карточек";

        return lastDigit switch
        {
            1 => "карточку",
            2 or 3 or 4 => "карточки",
            _ => "карточек"
        };
    }

    /// <summary>
    /// Склонение слова "день"
    /// Примеры: 1 день, 2 дня, 5 дней, 21 день
    /// </summary>
    private static string GetDayWord(int count)
    {
        var lastDigit = count % 10;
        var lastTwoDigits = count % 100;

        // Исключение для чисел 11-14
        if (lastTwoDigits >= 11 && lastTwoDigits <= 14)
            return "дней";

        return lastDigit switch
        {
            1 => "день",
            2 or 3 or 4 => "дня",
            _ => "дней"
        };
    }

    /// <summary>
    /// Склонение слова "час"
    /// Примеры: 1 час, 2 часа, 5 часов, 21 час
    /// </summary>
    private static string GetHourWord(int count)
    {
        var lastDigit = count % 10;
        var lastTwoDigits = count % 100;

        // Исключение для чисел 11-14
        if (lastTwoDigits >= 11 && lastTwoDigits <= 14)
            return "часов";

        return lastDigit switch
        {
            1 => "час",
            2 or 3 or 4 => "часа",
            _ => "часов"
        };
    }

    /// <summary>
    /// Склонение фразы "идеальная оценка"
    /// Примеры: 1 идеальную оценку, 2 идеальные оценки, 5 идеальных оценок
    /// </summary>
    private static string GetPerfectRatingWord(int count)
    {
        var lastDigit = count % 10;
        var lastTwoDigits = count % 100;

        // Исключение для чисел 11-14
        if (lastTwoDigits >= 11 && lastTwoDigits <= 14)
            return "идеальных оценок";

        return lastDigit switch
        {
            1 => "идеальную оценку",
            2 or 3 or 4 => "идеальные оценки",
            _ => "идеальных оценок"
        };
    }

    #endregion
}