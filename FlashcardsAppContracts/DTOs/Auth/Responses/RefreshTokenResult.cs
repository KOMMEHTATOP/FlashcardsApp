namespace FlashcardsAppContracts.DTOs.Auth.Responses;

public class RefreshTokenResult
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
