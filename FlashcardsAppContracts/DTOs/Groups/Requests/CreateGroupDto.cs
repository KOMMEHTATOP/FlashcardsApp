namespace FlashcardsAppContracts.DTOs.Groups.Requests;

public class CreateGroupDto
{
    public required string Name { get; set; }
    public required string Color { get; set; }
    public int Order { get; set; }

}
