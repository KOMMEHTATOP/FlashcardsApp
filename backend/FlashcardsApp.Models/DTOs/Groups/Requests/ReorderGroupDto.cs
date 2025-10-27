namespace FlashcardsApp.Models.DTOs.Groups.Requests;

public class ReorderGroupDto
{
    public Guid Id { get; set; }
    public int Order { get; set; }
}
