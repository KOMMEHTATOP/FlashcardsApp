using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.Models.DTOs.Auth.Requests;
using FlashcardsApp.Models.DTOs.Auth.Responses;
using FlashcardsApp.Models.DTOs.Requests;
using FlashcardsApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthBL _authBl;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        
        public AuthController(
            IAuthBL authBl,
            ITokenService tokenService,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            _authBl = authBl;
            _tokenService = tokenService;
            _configuration = configuration;
            _environment = environment;
        }

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var result = await _authBl.Register(model);
            
            if (!result.IsSuccess)
            {
                return BadRequest(new { errors = result.Errors });
            }
            
            return Ok(result.Data);
        }

        /// <summary>
        /// Вход пользователя
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var result = await _authBl.Login(model);
    
            if (!result.IsSuccess)
            {
                return Unauthorized(new { errors = result.Errors });
            }

            var user = result.Data!;
    
            var accessToken = await _tokenService.GenerateAccessToken(user);
            
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            var refreshToken = await _tokenService.GenerateRefreshToken(user.Id, ipAddress, userAgent);

            SetRefreshTokenCookie(refreshToken.Token);

            return Ok(new LoginResponseDto
            {
                AccessToken = accessToken,
                UserId = user.Id,
                Email = user.Email,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_configuration.GetValue<double>("Jwt:AccessTokenExpirationMinutes"))
            });
        }

        /// <summary>
        /// Обновить токен
        /// </summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshTokenValue))
            {
                return Unauthorized(new { errors = new[] { "Refresh токен не найден" } });
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            var result = await _authBl.RefreshAccessToken(refreshTokenValue, ipAddress, userAgent);
            
            if (!result.IsSuccess)
            {
                return Unauthorized(new { errors = result.Errors });
            }

            SetRefreshTokenCookie(result.Data!.RefreshToken);

            return Ok(new { accessToken = result.Data.AccessToken });
        }

        /// <summary>
        /// Выйти и отозвать токен обновления
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            Request.Cookies.TryGetValue("refreshToken", out var refreshTokenValue);

            await _authBl.Logout(refreshTokenValue);

            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = !_environment.IsDevelopment(),
                SameSite = _environment.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.None,
                Path = "/"
            });

            return Ok(new { message = "Выход выполнен успешно" });
        }
        
        
        /// <summary>
        /// Легкий запрос, ничего не делает. Нужно для получения токена
        /// </summary>
        [HttpGet("validate")]
        [Authorize]
        public IActionResult Validate()
        {
            return Ok(new { valid = true });
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var expirationDays = _configuration.GetValue<double>("Jwt:RefreshTokenExpirationDays");

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = !_environment.IsDevelopment(),
                SameSite = _environment.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(expirationDays),
                Path = "/"
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}