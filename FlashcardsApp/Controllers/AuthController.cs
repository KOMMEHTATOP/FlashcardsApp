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
        private readonly UserStatisticsService _statisticsService;
        
        public AuthController(
            UserManager<User> userManager, 
            SignInManager<User> signInManager,
            TokenService tokenService,
            IConfiguration configuration,
            UserStatisticsService statisticsService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _configuration = configuration;
            _statisticsService = statisticsService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var user = new User
            {
                Login = model.Login,
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Создаем начальную статистику
                await _statisticsService.CreateInitialStatisticsAsync(user.Id);
        
                return Ok(new RegisterUserDto
                {
                    IsSuccess = true,
                    Message = "Пользователь успешно зарегистрирован!"
                });
            }

            return BadRequest(new RegisterUserDto
            {
                IsSuccess = false,
                Message = "Ошибка регистрации",
                Errors = result.Errors.Select(e => TranslateIdentityError(e.Code, e.Description)).ToList()
            });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Неверный email или пароль" });
            }

            var passwordValid = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!passwordValid.Succeeded)
            {
                return Unauthorized(new { message = "Неверный email или пароль" });
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
                accessToken,
                user.Login,
                user.UserName,
                user.Email
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshTokenValue))
            {
                return Unauthorized(new { message = "Refresh токен не найден" });
            }

            var refreshToken = await _tokenService.ValidateRefreshToken(refreshTokenValue);
            if (refreshToken == null)
            {
                return Unauthorized(new { message = "Недействительный или истекший refresh токен" });
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

        private string TranslateIdentityError(string code, string originalDescription)
        {
            return code switch
            {
                "DuplicateUserName" => "Пользователь с таким именем уже существует",
                "DuplicateEmail" => "Пользователь с таким email уже зарегистрирован",
                "InvalidEmail" => "Некорректный email адрес",
                "InvalidUserName" => "Некорректное имя пользователя",
                "PasswordTooShort" => "Пароль должен содержать минимум 6 символов",
                "PasswordRequiresNonAlphanumeric" => "Пароль должен содержать специальные символы",
                "PasswordRequiresDigit" => "Пароль должен содержать цифры",
                "PasswordRequiresLower" => "Пароль должен содержать строчные буквы",
                "PasswordRequiresUpper" => "Пароль должен содержать заглавные буквы",
                "PasswordMismatch" => "Неверный пароль",
                "UserAlreadyHasPassword" => "У пользователя уже установлен пароль",
                "UserLockoutNotEnabled" => "Блокировка пользователя не включена",
                "UserAlreadyInRole" => "Пользователь уже имеет эту роль",
                "UserNotInRole" => "Пользователь не имеет этой роли",
                "LoginAlreadyAssociated" => "Этот вход уже привязан к другому пользователю",
                "InvalidToken" => "Недействительный токен",
                "RecoveryCodeRedemptionFailed" => "Не удалось использовать код восстановления",
                "ConcurrencyFailure" => "Ошибка параллельного доступа, попробуйте снова",
                "DefaultError" => "Произошла неизвестная ошибка",
                _ => originalDescription 
            };
        }
    }
}