namespace FlashcardsAppContracts.DTOs.Responses;

public class StudyHistoryDto
{
    public Guid Id { get; set; }
    public Guid CardId { get; set; }
    public string CardQuestion { get; set; } = string.Empty;
    public int Rating { get; set; }
    public int XPEarned { get; set; }
    public DateTime StudiedAt { get; set; }
}
