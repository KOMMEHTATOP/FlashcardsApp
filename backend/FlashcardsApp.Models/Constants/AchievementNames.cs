namespace FlashcardsApp.Models.Constants;

/// <summary>
/// Названия достижений (должны совпадать с данными в базе)
/// Используются для проверки условий разблокировки на бэкенде
/// и для отображения прогресса на фронтенде
/// </summary>
public static class AchievementNames
{
    /// <summary>
    /// "Первые шаги" - разблокируется при достижении 10 XP
    /// </summary>
    public const string FirstSteps = "Первые шаги";
    
    /// <summary>
    /// "7 дней подряд" - разблокируется при текущем streak >= 7 дней
    /// </summary>
    public const string SevenDayStreak = "7 дней подряд";
    
    /// <summary>
    /// "Неделя активности" - разблокируется при лучшем streak >= 7 дней
    /// </summary>
    public const string WeeklyActivity = "Неделя активности";
    
    /// <summary>
    /// "Король знаний" - разблокируется при достижении 10 уровня
    /// </summary>
    public const string KnowledgeKing = "Король знаний";
    
    /// <summary>
    /// "Восходящая звезда" - разблокируется при достижении 1000 XP
    /// </summary>
    public const string RisingStar = "Восходящая звезда";
}

/// <summary>
/// Критерии разблокировки достижений
/// Изменение этих значений влияет на логику начисления достижений
/// </summary>
public static class AchievementCriteria
{
    /// <summary>
    /// Необходимое количество XP для достижения "Первые шаги"
    /// </summary>
    public const int FirstStepsXP = 10;
    
    /// <summary>
    /// Необходимое количество дней streak для достижения "7 дней подряд"
    /// </summary>
    public const int SevenDayStreakDays = 7;
    
    /// <summary>
    /// Необходимое количество дней в лучшем streak для достижения "Неделя активности"
    /// </summary>
    public const int WeeklyActivityDays = 7;
    
    /// <summary>
    /// Необходимый уровень для достижения "Король знаний"
    /// </summary>
    public const int KnowledgeKingLevel = 10;
    
    /// <summary>
    /// Необходимое количество XP для достижения "Восходящая звезда"
    /// </summary>
    public const int RisingStarXP = 1000;
}