using FlashcardsApp.Models.Constants;

namespace FlashcardsApp.Models.DTOs.Responses;

public class ResultSettingsDto
{
    public StudyOrder StudyOrder { get; set; } = StudyOrder.Random;
    public int MinRating { get; set; }
    public int MaxRating { get; set; }
    public int CompletionThreshold { get; set; } = 4;
    public bool ShuffleOnRepeat { get; set; } = true;
}
