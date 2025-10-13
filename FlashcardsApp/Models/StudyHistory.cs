namespace FlashcardsApp.Models;

public class StudyHistory
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CardId { get; set; }
    public int Rating { get; set; }  // Оценка от 1 до 5
    public int XPEarned { get; set; }  // Сколько XP получил за эту карточку
    public DateTime StudiedAt { get; set; }
    
    // Навигационные свойства
    public User? User { get; set; }
    public Card? Card { get; set; }

}
