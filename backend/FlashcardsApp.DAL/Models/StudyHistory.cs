namespace FlashcardsApp.DAL.Models;

public class StudyHistory
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CardId { get; set; }
    public int Rating { get; set; }  
    public int XPEarned { get; set; }  
    public DateTime StudiedAt { get; set; }
    public User? User { get; set; }
    public Card? Card { get; set; }

}
