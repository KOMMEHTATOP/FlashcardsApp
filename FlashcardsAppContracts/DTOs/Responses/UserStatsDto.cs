namespace FlashcardsAppContracts.DTOs.Responses;

public class UserStatsDto
{
    /// <summary>
    /// Общий опыт пользователя
    /// </summary>
    public int TotalXP { get; set; }
    
    /// <summary>
    /// Текущий уровень
    /// </summary>
    public int Level { get; set; }
    
    /// <summary>
    /// Сколько XP осталось до следующего уровня
    /// </summary>
    public int XPForNextLevel { get; set; }
    
    /// <summary>
    /// Прогресс в текущем уровне (сколько набрано)
    /// </summary>
    public int XPProgressInCurrentLevel { get; set; }
    
    /// <summary>
    /// Требуется для текущего уровня (всего)
    /// </summary>
    public int XPRequiredForCurrentLevel { get; set; }
    
    /// <summary>
    /// Текущая серия дней подряд
    /// </summary>
    public int CurrentStreak { get; set; }
    
    /// <summary>
    /// Лучшая серия дней подряд
    /// </summary>
    public int BestStreak { get; set; }
    
    /// <summary>
    /// Общее время обучения
    /// </summary>
    public TimeSpan TotalStudyTime { get; set; }

    /// <summary>
    /// Всего изучено карточек
    /// </summary>
    public int TotalCardsStudied { get; set; }
    
    /// <summary>
    /// Всего создано карточек
    /// </summary>
    public int TotalCardsCreated { get; set; }
    
    /// <summary>
    /// Серия идеальных оценок (5 звезд подряд)
    /// </summary>
    public int PerfectRatingsStreak { get; set; }
}
