using System.ComponentModel.DataAnnotations;

namespace FlashcardsApp.Models.DTOs;

public class CreateCardRatingDto
{
    [Range(1,5, ErrorMessage = "Rating must be between 1 and 5")]
    public required int Rating { get; set; }
}
