namespace FlashcardsApp.Models.DTOs.Groups.Requests;

public class ChangeAccessGroupDto
{
    public Guid GroupId { get; set; }
    public bool IsPublished { get; set; }
}


