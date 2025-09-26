using FlashcardsAppContracts.Constants;
using System.ComponentModel.DataAnnotations;

namespace FlashcardsAppContracts.DTOs.Requests;

public class CreateSettingsDto
{
    public Guid? GroupId { get; set; }
    public StudyOrder StudyOrder { get; set; } = StudyOrder.Random;
    [MaxLength(100)]
    public string? PresetName { get; set; }

    [Range(0,5)]
    public int MinRating { get; set; }
    [Range(0,5)]
    public int MaxRating { get; set; }
}
