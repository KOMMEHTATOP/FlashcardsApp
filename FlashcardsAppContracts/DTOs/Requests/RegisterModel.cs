namespace FlashcardsAppContracts.DTOs.Requests;

public class RegisterModel
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
