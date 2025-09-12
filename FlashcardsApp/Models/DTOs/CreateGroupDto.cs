using FlashcardsApp.Constants;

namespace FlashcardsApp.Models.DTOs;

public class CreateGroupDto
{
    public required string Name { get; set; }
    public required GroupColor Color { get; set; }
}
