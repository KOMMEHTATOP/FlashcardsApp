namespace FlashcardsApp.Models.Enums;

/// <summary>
/// Редкость достижения (влияет на UI и награды)
/// </summary>
public enum AchievementRarity
{
    /// <summary>
    /// Обычное достижение (легко получить)
    /// </summary>
    Common = 1,
    
    /// <summary>
    /// Редкое достижение
    /// </summary>
    Rare = 2,
    
    /// <summary>
    /// Эпическое достижение
    /// </summary>
    Epic = 3,
    
    /// <summary>
    /// Легендарное достижение (очень сложно получить)
    /// </summary>
    Legendary = 4
}
