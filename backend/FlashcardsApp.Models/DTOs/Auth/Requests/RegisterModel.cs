using System.ComponentModel.DataAnnotations;

namespace FlashcardsApp.Models.DTOs.Auth.Requests;

public class RegisterModel
{
    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный формат email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Логин обязателен")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Логин должен быть от 3 до 50 символов")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Логин может содержать только буквы, цифры и подчеркивания")]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен")]
    [MinLength(6, ErrorMessage = "Пароль должен быть не менее 6 символов")]
    public string Password { get; set; } = string.Empty;

    public string Role { get; set; } = "User"; 
}
