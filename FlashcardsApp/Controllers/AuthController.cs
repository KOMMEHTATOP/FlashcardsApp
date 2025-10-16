using FlashcardsApp.Interfaces;
using FlashcardsAppContracts.DTOs.Auth.Requests;
using FlashcardsAppContracts.DTOs.Auth.Responses;
using FlashcardsAppContracts.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        
        public AuthController(
            IAuthService authService,
            ITokenService tokenService,
            IConfiguration configuration)
        {
            _authService = authService;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var result = await _authService.Register(model);
            
            if (!result.IsSuccess)
            {
                return BadRequest(new { errors = result.Errors });
            }
            
            return Ok(result.Data);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var result = await _authService.Login(model);
    
            if (!result.IsSuccess)
            {
                return Unauthorized(new { message = string.Join(", ", result.Errors) });
            }

            var user = result.Data!;
    
            // HTTP-специфичная логика: извлечение данных из запроса
            var accessToken = _tokenService.GenerateAccessToken(user);
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            var refreshToken = await _tokenService.GenerateRefreshToken(user.Id, ipAddress, userAgent);

            // HTTP-специфичная логика: установка cookie
            SetRefreshTokenCookie(refreshToken.Token);

            return Ok(new LoginResponseDto
            {
                AccessToken = accessToken,
                UserId = user.Id,
                Email = user.Email,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_configuration.GetValue<double>("Jwt:AccessTokenExpirationMinutes"))
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            // HTTP-специфичная логика: чтение cookie
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshTokenValue))
            {
                return Unauthorized(new { message = "Refresh токен не найден" });
            }

            // HTTP-специфичная логика: извлечение данных из запроса
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            // Бизнес-логика в сервисе
            var result = await _authService.RefreshAccessToken(refreshTokenValue, ipAddress, userAgent);
            
            if (!result.IsSuccess)
            {
                return Unauthorized(new { message = string.Join(", ", result.Errors) });
            }

            // HTTP-специфичная логика: установка cookie
            SetRefreshTokenCookie(result.Data!.RefreshToken);

            return Ok(new { accessToken = result.Data.AccessToken });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // HTTP-специфичная логика: чтение cookie
            Request.Cookies.TryGetValue("refreshToken", out var refreshTokenValue);

            // Бизнес-логика в сервисе
            await _authService.Logout(refreshTokenValue);

            // HTTP-специфичная логика: удаление cookie
            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });

            return Ok(new { message = "Выход выполнен успешно" });
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var expirationDays = _configuration.GetValue<double>("Jwt:RefreshTokenExpirationDays");

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(expirationDays),
                Path = "/"
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}