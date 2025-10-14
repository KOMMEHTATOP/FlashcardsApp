using System.ComponentModel.DataAnnotations;

namespace FlashcardsAppContracts.DTOs.Requests;

public class RecordStudyDto
{
    [Required(ErrorMessage = "CardId is required")]
    public Guid CardId { get; set; }
    [Required(ErrorMessage = "Rating is required")]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }  
}
