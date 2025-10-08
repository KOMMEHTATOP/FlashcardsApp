using System.ComponentModel.DataAnnotations;

namespace FlashcardsApp.Models;

public class RefreshToken
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    [Required]
    [MaxLength(500)]
    public string Token { get; set; } = string.Empty;
    
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Для отзыва токена при logout
    public bool IsRevoked { get; set; }
    
    // Опционально: для отслеживания откуда пришел запрос
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}
