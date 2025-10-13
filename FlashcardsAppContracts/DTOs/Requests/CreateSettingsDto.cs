using FlashcardsAppContracts.Constants;
using System.ComponentModel.DataAnnotations;

namespace FlashcardsAppContracts.DTOs.Requests;

public class CreateSettingsDto
{
    public StudyOrder StudyOrder { get; set; } = StudyOrder.Random;

    [Range(0,5)]
    public int MinRating { get; set; }
    [Range(0,5)]
    public int MaxRating { get; set; }
    [Range(1, 5)]
    public int CompletionThreshold { get; set; } = 5;
    public bool ShuffleOnRepeat { get; set; } = true;

}
