namespace FlashcardsApp.Models.DTOs.Tags;

public class GroupTagDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string? Color { get; set; }
}
