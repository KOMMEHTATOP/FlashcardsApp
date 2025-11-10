namespace FlashcardsApp.Models.DTOs.Groups.Requests;

public class CreateGroupDto
{
    public required string Name { get; set; }
    public required string Color { get; set; }
    public string GroupIcon { get; set; }
    public int Order { get; set; }
    public bool IsPublished { get; set; }

}
