namespace FlashcardsApp.Models.DTOs.Admin;

public class AdminUserDto
{
    public Guid Id { get; set; }
    public string Login { get; set; } = string.Empty; 
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;  
    public int TotalRating { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastLogin { get; set; }
    
    // Статистика (вычисляемая)
    public int GroupsCount { get; set; }
    public int CardsCount { get; set; }
}
