namespace FlashcardsApp.Models.Enums;

/// <summary>
/// Типы условий для разблокировки достижений
/// </summary>
public enum AchievementConditionType
{
    /// <summary>
    /// Общее количество изученных карточек
    /// </summary>
    TotalCardsStudied = 1,
    
    /// <summary>
    /// Общее количество созданных карточек
    /// </summary>
    TotalCardsCreated = 2,
    
    /// <summary>
    /// Текущая серия (streak) в днях
    /// </summary>
    CurrentStreak = 3,
    
    /// <summary>
    /// Лучшая серия за все время
    /// </summary>
    BestStreak = 4,
    
    /// <summary>
    /// Достижение уровня
    /// </summary>
    Level = 5,
    
    /// <summary>
    /// Общее количество XP
    /// </summary>
    TotalXP = 6,
    
    /// <summary>
    /// Серия идеальных оценок (5) подряд
    /// </summary>
    PerfectRatingsStreak = 7,
    
    /// <summary>
    /// Общее время обучения (в часах)
    /// </summary>
    TotalStudyTimeHours = 8
}
