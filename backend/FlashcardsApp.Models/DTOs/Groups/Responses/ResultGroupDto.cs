using FlashcardsApp.Models.DTOs.Tags;

namespace FlashcardsApp.Models.DTOs.Groups.Responses;

public class ResultGroupDto
{
    public Guid Id { get; set; }
    public required string GroupName { get; set; }
    public required string GroupColor { get; set; }
    public required string GroupIcon { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Order { get; set; }
    public int SubscriberCount { get; set; }
    public int CardCount { get; set; }
    public List<GroupTagDto> Tags { get; set; } = new();
}
