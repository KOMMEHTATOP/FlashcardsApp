using FlashcardsAppContracts.Constants;
using System.ComponentModel.DataAnnotations;

namespace FlashcardsApp.Models;

public class StudySettings
{
    public required Guid StudySettingsId { get; set; }
    public required Guid UserId { get; set; }
    public Guid? GroupId { get; set; }
    public StudyOrder StudyOrder { get; set; } = StudyOrder.Random;
    
    [MaxLength(100)] 
    public string? PresetName { get; set; }

    public int MinRating { get; set; }
    public int MaxRating { get; set; }
    
    public int CompletionThreshold { get; set; } = 5;
    public bool ShuffleOnRepeat { get; set; } = true;

    public User? User { get; set; }
    public Group? Group { get; set; }
}
