using FlashcardsApp.Models.Constants;

namespace FlashcardsApp.Models.DTOs.Responses;

public class ResultSettingsDto
{
    public StudyOrder StudyOrder { get; set; }
    public int MinRating { get; set; }
    public int MaxRating { get; set; }
    public int CompletionThreshold { get; set; }
    public bool ShuffleOnRepeat { get; set; }


    public static ResultSettingsDto GetDefault()
    {
        return new ResultSettingsDto
        {
            StudyOrder = StudyOrder.Random,
            MinRating = 0,
            MaxRating = 5,
            CompletionThreshold = 5,
            ShuffleOnRepeat = true
        };
    }
}
