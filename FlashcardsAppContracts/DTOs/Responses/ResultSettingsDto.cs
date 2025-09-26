using FlashcardsAppContracts.Constants;

namespace FlashcardsAppContracts.DTOs.Responses;

public class ResultSettingsDto
{
    public Guid StudySettingsId { get; set; }
    public Guid? GroupId { get; set; }
    public StudyOrder StudyOrder { get; set; } = StudyOrder.Random;
    public string? PresetName { get; set; }

    public int MinRating { get; set; }
    public int MaxRating { get; set; }
    
    public int CompletionThreshold { get; set; } = 4;
    
    public bool ShuffleOnRepeat { get; set; } = true;
}
