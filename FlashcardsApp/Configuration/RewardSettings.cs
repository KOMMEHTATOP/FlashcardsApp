namespace FlashcardsApp.Configuration;

/// <summary>
/// Настройки системы наград: XP, монеты, множители
/// </summary>
public class RewardSettings
{
    public BaseRewards Base { get; set; } = new();
    public Multipliers Multipliers { get; set; } = new();
    public Limits Limits { get; set; } = new();
}

/// <summary>
/// Базовые награды за действия
/// </summary>
public class BaseRewards
{
    /// <summary>
    /// XP за изучение одной карточки (базовое значение, до применения множителей)
    /// </summary>
    public int XPPerCard { get; set; } = 10;
    
    /// <summary>
    /// XP за завершение сессии изучения
    /// </summary>
    public int XPPerSession { get; set; } = 50;
    
    /// <summary>
    /// XP за создание новой карточки
    /// </summary>
    public int XPPerCreatedCard { get; set; } = 10;
    
    /// <summary>
    /// XP за выполнение достижения
    /// </summary>
    public int XPPerAchievement { get; set; } = 100;
    
    /// <summary>
    /// Монеты за выполнение достижения (будущий функционал)
    /// </summary>
    public int CoinsPerAchievement { get; set; } = 20;
}

/// <summary>
/// Множители для расчета наград
/// </summary>
public class Multipliers
{
    /// <summary>
    /// Бонус за streak по пороговым значениям (не линейный)
    /// Применяется в методе CalculateStreakBonus() в GamificationService
    /// </summary>
    public StreakBonuses StreakBonus { get; set; } = new();
    
    /// <summary>
    /// Множитель качества ответа (применяется в CalculateQualityMultiplier)
    /// </summary>
    public QualityMultipliers Quality { get; set; } = new();
    
    /// <summary>
    /// Множитель сложности карточки (применяется в CalculateDifficultyMultiplierAsync)
    /// </summary>
    public DifficultyMultipliers Difficulty { get; set; } = new();
}

/// <summary>
/// Бонусы за streak (пороговые значения)
/// </summary>
public class StreakBonuses
{
    public double Days30Plus { get; set; } = 1.5;   // Месяц подряд
    public double Days14Plus { get; set; } = 1.25;  // Две недели
    public double Days7Plus { get; set; } = 1.1;    // Неделя
    public double Default { get; set; } = 1.0;      // Нет бонуса
}

/// <summary>
/// Множители качества ответа (рейтинг 1-5)
/// </summary>
public class QualityMultipliers
{
    public double Rating5 { get; set; } = 1.5;  // Отлично
    public double Rating4 { get; set; } = 1.2;  // Хорошо
    public double Rating3 { get; set; } = 1.0;  // Нормально
    public double Rating2 { get; set; } = 0.7;  // Плохо
    public double Rating1 { get; set; } = 0.5;  // Очень плохо
}

/// <summary>
/// Множители сложности карточки (по средней оценке из истории)
/// </summary>
public class DifficultyMultipliers
{
    public double Easy { get; set; } = 0.8;    // Средний рейтинг >= 4.0
    public double Medium { get; set; } = 1.0;  // Средний рейтинг >= 2.5
    public double Hard { get; set; } = 1.5;    // Средний рейтинг < 2.5
}

/// <summary>
/// Лимиты системы наград
/// </summary>
public class Limits
{
    /// <summary>
    /// Максимальное количество XP которое можно получить за день
    /// </summary>
    public int MaxDailyXP { get; set; } = 500;
}