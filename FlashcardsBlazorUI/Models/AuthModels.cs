namespace FlashcardsBlazorUI.Models
{
    public class LoginRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class LoginResponse
    {
        public string Token { get; set; } = "";
        public string UserId { get; set; } = "";
    }
}
