namespace FlashcardsApp.Models.DTOs.Groups.Requests;

public class ChangeAccessGroupDto
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public bool IsPublished { get; set; }
}


