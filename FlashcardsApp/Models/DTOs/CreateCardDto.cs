namespace FlashcardsApp.Models.DTOs;

public class CreateCardDto
{
    public required string Question { get; set; }
    public required string Answer { get; set; }
}
