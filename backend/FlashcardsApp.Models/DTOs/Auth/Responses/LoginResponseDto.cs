using System.ComponentModel.DataAnnotations;

namespace FlashcardsApp.Models.DTOs.Auth.Responses;

public class LoginResponseDto
{
    [Required]
    public required string AccessToken { get; set; }
    
    public Guid? UserId { get; set; }
    
    public string? Email { get; set; }
    
    public DateTime? ExpiresAt { get; set; }

}
