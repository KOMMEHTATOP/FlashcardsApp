using FlashcardsApp.Constants;

namespace FlashcardsApp.Models.DTOs;

public class ResultGroupDto
{
    public Guid Id {get; set;}
    public required string GroupName  {get; set;}
    public string GroupColor { get; set; } = GroupColors.Default;
    public DateTime CreatedAt { get; set; }
}
