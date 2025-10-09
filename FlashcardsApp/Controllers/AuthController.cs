using FlashcardsApp.Models;
using FlashcardsApp.Services;
using FlashcardsAppContracts.DTOs.Requests;
using FlashcardsAppContracts.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly TokenService _tokenService;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<User> userManager, 
            SignInManager<User> signInManager,
            TokenService tokenService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new RegisterUserDto
                {
                    IsSuccess = true,
                    Message = "User created successfully!"
                });
            }

            return BadRequest(new RegisterUserDto
            {
                IsSuccess = false,
                Message = "Registration failed",
                Errors = result.Errors.Select(e => e.Description).ToList()
            });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            var passwordValid = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!passwordValid.Succeeded)
            {
                return Unauthorized("Invalid email or password");
            }

            // Генерируем оба токена
            var accessToken = _tokenService.GenerateAccessToken(user);
            
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            var refreshToken = await _tokenService.GenerateRefreshToken(user.Id, ipAddress, userAgent);

            // Устанавливаем Refresh Token в httpOnly cookie
            SetRefreshTokenCookie(refreshToken.Token);

            return Ok(new
            {
                accessToken
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshTokenValue))
            {
                return Unauthorized("Refresh token not found");
            }

            var refreshToken = await _tokenService.ValidateRefreshToken(refreshTokenValue);
            if (refreshToken == null)
            {
                return Unauthorized("Invalid or expired refresh token");
            }

            var accessToken = _tokenService.GenerateAccessToken(refreshToken.User);

            // Refresh Token Rotation
            await _tokenService.RevokeRefreshToken(refreshTokenValue);
            
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            var newRefreshToken = await _tokenService.GenerateRefreshToken(refreshToken.UserId, ipAddress, userAgent);
            
            SetRefreshTokenCookie(newRefreshToken.Token);

            return Ok(new
            {
                accessToken
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            if (Request.Cookies.TryGetValue("refreshToken", out var refreshTokenValue))
            {
                await _tokenService.RevokeRefreshToken(refreshTokenValue);
            }

            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });

            return Ok(new { message = "Logged out successfully" });
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