using FlashcardsAppContracts.DTOs.Achievements.Responses;

namespace FlashcardsAppContracts.DTOs.Study.Responses;

/// <summary>
/// Награды и статистика после изучения карточки
/// </summary>
public class StudyRewardDto
{
    // ============ XP и Уровень ============
    
    /// <summary>
    /// Сколько XP заработано в этой сессии
    /// </summary>
    public int XPEarned { get; set; }
    
    /// <summary>
    /// Общее количество XP пользователя
    /// </summary>
    public int TotalXP { get; set; }
    
    /// <summary>
    /// Текущий уровень пользователя
    /// </summary>
    public int CurrentLevel { get; set; }
    
    /// <summary>
    /// Был ли повышен уровень (для показа анимации)
    /// </summary>
    public bool LeveledUp { get; set; }
    
    // ============ Прогресс до следующего уровня ============
    
    /// <summary>
    /// XP в текущем уровне (например, 38 из 100)
    /// </summary>
    public int CurrentLevelXP { get; set; }
    
    /// <summary>
    /// Сколько XP нужно для следующего уровня (например, 100)
    /// </summary>
    public int XPForNextLevel { get; set; }
    
    /// <summary>
    /// Сколько XP осталось до следующего уровня (например, 62)
    /// </summary>
    public int XPToNextLevel { get; set; }
    
    // ============ Streak ============
    
    /// <summary>
    /// Увеличился ли streak в эту сессию
    /// </summary>
    public bool StreakIncreased { get; set; }
    
    /// <summary>
    /// Текущий streak пользователя (дней подряд)
    /// </summary>
    public int CurrentStreak { get; set; }
    
    // ============ Достижения ============
    
    /// <summary>
    /// Новые разблокированные достижения в эту сессию
    /// </summary>
    public List<AchievementUnlockedDto> NewAchievements { get; set; } = new();
}
