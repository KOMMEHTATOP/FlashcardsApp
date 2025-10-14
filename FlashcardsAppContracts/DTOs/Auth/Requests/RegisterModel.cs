using System.ComponentModel.DataAnnotations;

namespace FlashcardsAppContracts.DTOs.Requests;

public class RegisterModel
{
    [Required(ErrorMessage = "Login обязателен")]
    public required string Login { get; set; }
    
    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный формат email")]
    public required string Email { get; set; }
    
    [Required(ErrorMessage = "Пароль обязателен")]
    [MinLength(6, ErrorMessage = "Пароль должен быть не менее 6 символов")]
    public required string Password { get; set; }
}
