namespace FlashcardsAppContracts.DTOs.Responses;

public class RegisterUserDto
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
}
