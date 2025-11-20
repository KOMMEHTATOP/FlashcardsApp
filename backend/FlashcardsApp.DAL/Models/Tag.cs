using System.ComponentModel.DataAnnotations;

namespace FlashcardsApp.DAL.Models;

public class Tag
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Name { get; set; } // Например: "Программирование"

    [Required]
    [MaxLength(50)]
    public required string Slug { get; set; } // Например: "programming" (для URL)

    [MaxLength(20)]
    public string? Color { get; set; } // Например: "blue", "#ff0000" или gradient class

    // Навигационное свойство для связи Many-to-Many
    public List<Group> Groups { get; set; } = new();
}
