using FlashcardsAppContracts.Constants;

namespace FlashcardsApp.Models;

public class StudySettings
{
    public required Guid SettingsId { get; set;}
    public required Guid UserId { get; set; }
    public Guid? GroupId { get; set; }
    public StudyOrder StudyOrder { get; set; } = StudyOrder.Random;
    public string? PresetName { get; set; }

    public int MinRating { get; set; }
    public int MaxRating { get; set; }

    public User? User { get; set; }
    public Group? Group { get; set; }
}
