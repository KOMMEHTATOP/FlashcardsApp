using System.ComponentModel.DataAnnotations;

namespace FlashcardsApp.Models.DTOs.Import.Requests;

public class ImportGroupWithCardsDto
{
    [Required(ErrorMessage = "Group name is required")]
    [MaxLength(100, ErrorMessage = "Group name cannot exceed 100 characters")]
    public required string GroupName { get; set; }
    
    [Required(ErrorMessage = "Group color is required")]
    public required string GroupColor { get; set; }
    
    [Required(ErrorMessage = "Group icon is required")]
    public required string GroupIcon { get; set; }
    
    [Range(0, int.MaxValue)]
    public int Order { get; set; }
    
    [Required(ErrorMessage = "Cards array is required")]
    [MaxLength(100, ErrorMessage = "Cannot import more than 100 cards at once")]
    [MinLength(1, ErrorMessage = "At least one card is required")]
    public required List<ImportCardDto> Cards { get; set; } = new();
    public bool IsPublished { get; set; }
}

public class ImportCardDto
{
    [Required(ErrorMessage = "Question is required")]
    [MaxLength(500, ErrorMessage = "Question cannot exceed 500 characters")]
    public required string Question { get; set; }
    
    [Required(ErrorMessage = "Answer is required")]
    [MaxLength(1000, ErrorMessage = "Answer cannot exceed 1000 characters")]
    public required string Answer { get; set; }
    public bool IsPublished { get; set; }
}
