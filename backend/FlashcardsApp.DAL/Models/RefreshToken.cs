using System.ComponentModel.DataAnnotations;

namespace FlashcardsApp.DAL.Models;

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
    public DateTime? RevokedAt { get; set; }
    public string? RevokedByIp { get; set; }
    
    // Для отслеживания откуда пришел запрос
    public string? CreatedByIp { get; set; }
    public string? CreatedByUserAgent { get; set; }
    
    // Старые поля для совместимости (можно удалить если не используются)
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}
