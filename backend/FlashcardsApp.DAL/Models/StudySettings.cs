using FlashcardsApp.Models.Constants;

namespace FlashcardsApp.DAL.Models;

public class StudySettings
{
    public Guid StudySettingsId { get; set; }
    public required Guid UserId { get; set; }
    public StudyOrder StudyOrder { get; set; } = StudyOrder.Random;
    public int MinRating { get; set; } = 0;
    public int MaxRating { get; set; } = 5;
    public int CompletionThreshold { get; set; } = 5;
    public bool ShuffleOnRepeat { get; set; } = true;
    public User? User { get; set; }

}
