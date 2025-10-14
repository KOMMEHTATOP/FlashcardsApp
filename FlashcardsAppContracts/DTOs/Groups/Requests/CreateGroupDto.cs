using FlashcardsAppContracts.Constants;

namespace FlashcardsAppContracts.DTOs.Requests;

public class CreateGroupDto
{
    public required string Name { get; set; }
    public required GroupColor Color { get; set; }
    public int Order { get; set; }

}
