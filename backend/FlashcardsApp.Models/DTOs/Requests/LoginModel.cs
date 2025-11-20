namespace FlashcardsApp.Models.DTOs.Requests;

public class LoginModel
{
    public required string Email {get; set;}
    public required string Password {get; set;}
    public DateTime LastLogin {get; set;}
}
