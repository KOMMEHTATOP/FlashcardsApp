using System.ComponentModel.DataAnnotations;

namespace FlashcardsApp.DAL.Models;

public class Tag
{
    public Guid Id { get; set; }
    [Required]
    [MaxLength(50)]
    public required string Name { get; set; } 
    [Required]
    [MaxLength(50)]
    public required string Slug { get; set; } 
    [MaxLength(20)]
    public string? Color { get; set; } 
    public List<Group> Groups { get; set; } = new();
}
