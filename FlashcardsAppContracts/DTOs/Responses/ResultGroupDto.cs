using FlashcardsAppContracts.Constants;

namespace FlashcardsAppContracts.DTOs.Responses;

public class ResultGroupDto
{
    public Guid Id {get; set;}
    public required string GroupName  {get; set;}
    public required GroupColor GroupColor { get; set; } = GroupColor.Green;
    public DateTime CreatedAt { get; set; }
    public int Order { get; set; }
}
