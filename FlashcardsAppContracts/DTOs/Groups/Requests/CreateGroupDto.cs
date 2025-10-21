namespace FlashcardsAppContracts.DTOs.Groups.Requests;

public class CreateGroupDto
{
    public required string Name { get; set; }
    public required string Color { get; set; }
    public string Icon { get; set; }
    public int Order { get; set; }

}
