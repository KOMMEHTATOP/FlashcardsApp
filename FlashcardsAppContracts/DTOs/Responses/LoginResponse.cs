using System.ComponentModel.DataAnnotations;

namespace FlashcardsAppContracts.DTOs.Responses;

public class LoginResponse
{
    [Required]
    public required string Token { get; set; }
    
    public Guid? UserId { get; set; }
    
    public string? Email { get; set; }
    
    public DateTime? ExpiresAt { get; set; }

}
