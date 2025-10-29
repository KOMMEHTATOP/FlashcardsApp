using FlashcardsApp.DAL;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Services.Implementations;
using FlashcardsApp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace FlashcardsApp.Tests.Unit.Services;

public class TokenServiceTests
{
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    private const string TestJwtKey = "super-secret-test-key-minimum-32-characters-long-for-security";
    private const string TestIssuer = "TestIssuer";
    private const string TestAudience = "TestAudience";
    private const int TestAccessTokenExpiration = 60; // минуты 

    public TokenServiceTests()
    {
        // Arrange: Настраиваем конфигурацию (один раз для всех тестов)
        var configurationData = new Dictionary<string, string>
        {
            { "Jwt:Key", TestJwtKey },
            { "Jwt:Issuer", TestIssuer },
            { "Jwt:Audience", TestAudience },
            { "Jwt:AccessTokenExpirationMinutes", TestAccessTokenExpiration.ToString() },
            { "Jwt:RefreshTokenExpirationDays", "30" }
        };
        
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData!)
            .Build();
        
        // Создаем моки для зависимостей которые не нужны в unit-тестах
        var mockContext = CreateMockDbContext();
        var mockUserManager = CreateMockUserManager();

        // Создаем TokenService для тестирования
        _tokenService = new TokenService(_configuration, mockContext, mockUserManager);

    }
    
    // Вспомогательный метод: создаем мок DbContext
    private static ApplicationDbContext CreateMockDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Уникальная БД для каждого теста
            .Options;

        return new ApplicationDbContext(options);
    }

    // Вспомогательный метод: создаем мок UserManager
    private static UserManager<User> CreateMockUserManager()
    {
        var userStoreMock = new Mock<IUserStore<User>>();
        return new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null
        ).Object;
    }
}
