using FlashcardsApp.Data;
using FlashcardsApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FlashcardsApp.Services;

public class TokenService
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;

    public TokenService(IConfiguration configuration, ApplicationDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    // Генерация Access Token (JWT, 30 минут)
    public string GenerateAccessToken(User user)
    {
        var expirationMinutes = _configuration.GetValue<double>("Jwt:AccessTokenExpirationMinutes");

        var jwtKey = _configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT key is not configured");
        }

        var key = Encoding.ASCII.GetBytes(jwtKey);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes), 
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    // Генерация Refresh Token (случайная строка, 7 дней)
    public async Task<RefreshToken> GenerateRefreshToken(Guid userId, string? ipAddress, string? userAgent)
    {
        var expirationDays = _configuration.GetValue<double>("Jwt:RefreshTokenExpirationDays");
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = GenerateRandomToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(expirationDays),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false,
            IpAddress = ipAddress,
            UserAgent = userAgent
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return refreshToken;
    }

    // Генерация криптографически стойкой случайной строки
    private string GenerateRandomToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    // Валидация Refresh Token
    public async Task<RefreshToken?> ValidateRefreshToken(string token)
    {
        var refreshToken = await _context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == token);

        if (refreshToken == null) return null;
        if (refreshToken.IsRevoked) return null;
        if (refreshToken.ExpiresAt < DateTime.UtcNow) return null;

        return refreshToken;
    }

    // Отзыв Refresh Token
    public async Task<bool> RevokeRefreshToken(string token)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == token);

        if (refreshToken == null) return false;

        refreshToken.IsRevoked = true;
        await _context.SaveChangesAsync();
        return true;
    }

    // Очистка старых токенов (можно запускать по расписанию)
    public async Task CleanupExpiredTokens()
    {
        var expiredTokens = await _context.RefreshTokens
            .Where(r => r.ExpiresAt < DateTime.UtcNow)
            .ToListAsync();

        _context.RefreshTokens.RemoveRange(expiredTokens);
        await _context.SaveChangesAsync();
    }
}