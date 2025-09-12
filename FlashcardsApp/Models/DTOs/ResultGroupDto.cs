using FlashcardsApp.Constants;

namespace FlashcardsApp.Models.DTOs;

public class ResultGroupDto
{
    public Guid Id {get; set;}
    public required string GroupName  {get; set;}
    public required GroupColor GroupColor { get; set; } = GroupColor.Green;
    public DateTime CreatedAt { get; set; }
}
