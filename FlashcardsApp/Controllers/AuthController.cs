using FlashcardsApp.Models;
using FlashcardsAppContracts.DTOs.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FlashcardsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var user = new User
            {
                UserName = model.Email, Email = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new
                {
                    Message = "User created successfully"
                });
            }

            return BadRequest(result.Errors);
        }

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

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                token
            });
        }


        private string GenerateJwtToken(User user)
        {
            //Тот же самый секретный ключ что и в programs
            var jwtKey = "your-super-secret-key-at-least-32-characters-long!"; // тот же ключ из Program.cs
            var key = Encoding.ASCII.GetBytes(jwtKey);

            //НАстройка полей для токена - упаковывает данные пользователя
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // уникальный ID токена
            };

            //Описывает параметры токена
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), // Кто владелец токена - упаковывает все claims (UserId, Email и т.д.)
                Expires = DateTime.UtcNow.AddSeconds(40), // Время жизни токена
                SigningCredentials = new SigningCredentials( // Как подписать токен для защиты от подделки
                    new SymmetricSecurityKey(key), // Секретный ключ для создания цифровой подписи
                    SecurityAlgorithms.HmacSha256) // Алгоритм шифрования подписи (HMAC-SHA256 - стандарт)
            };

            var tokenHandler = new JwtSecurityTokenHandler(); // Инструмент для создания и чтения JWT токенов
            var token = tokenHandler.CreateToken(tokenDescriptor); // Создает токен по описанию выше
            return tokenHandler.WriteToken(token); // Превращает токен в строку для отправки клиенту
        }
    }
}
