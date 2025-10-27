using FlashcardsApp.Api.Hubs;
using FlashcardsApp.Api.Infrastructure.Notifications;
using FlashcardsApp.Api.Resources;
using FlashcardsApp.BLL.Implementations;
using FlashcardsApp.BLL.Implementations.Achievements;
using FlashcardsApp.BLL.Interfaces;
using FlashcardsApp.BLL.Interfaces.Achievements;
using FlashcardsApp.DAL.Data;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.Constants;
using FlashcardsApp.Services.Implementations;
using FlashcardsApp.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Добавляем логирование
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var logger = LoggerFactory.Create(config => { config.AddConsole(); }).CreateLogger("Startup");

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    logger.LogCritical("❌ Connection string is not configured.");
    throw new Exception("Connection string is not configured.");
}

builder.Services.AddLocalization();

var supportedCultures = new[]
{
    new CultureInfo("ru"),
    new CultureInfo("en")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("ru");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.FallBackToParentUICultures = true;
});

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options
    => options.UseNpgsql(connectionString));

// Identity с локализацией
builder.Services.AddIdentity<User, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddErrorDescriber<LocalizedIdentityErrorDescriber>()
    .AddDefaultTokenProviders();

// JWT
var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrEmpty(jwtKey))
{
    logger.LogCritical("❌ JWT key is not configured. Please set Jwt:Key in configuration.");
    throw new Exception("JWT key is not configured. Please set Jwt:Key in configuration.");
}

var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        // Добавляем поддержку JWT для SignalR
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/notifications"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

// SignalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
});

// Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FlashcardsApp API",
        Version = "v1",
        Description = "API для приложения изучения карточек"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                }
            },
            Array.Empty<string>()
        }
    });
});

// Настройки наград
builder.Services.Configure<RewardSettings>(
    builder.Configuration.GetSection("RewardSettings"));

// Регистрация сервисов
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<IStudySettingsService, StudySettingsService>();
builder.Services.AddScoped<IStudySessionService, StudySessionService>();
builder.Services.AddScoped<IUserStatisticsService, UserStatisticsService>();

// Gamification
builder.Services.AddScoped<IGamificationService, GamificationService>();
builder.Services.AddScoped<IStudyService, StudyService>();

// NotificationService
builder.Services.AddScoped<INotificationService, SignalRNotificationService>();

// Achievement services
builder.Services.AddScoped<IAchievementService, AchievementService>();
builder.Services.AddScoped<IAchievementRewardService, AchievementRewardService>();
builder.Services.AddScoped<IAchievementLeaderboardService, AchievementLeaderboardService>();
builder.Services.AddScoped<IAchievementProgressService, AchievementProgressService>();
builder.Services.AddScoped<IAchievementMotivationService, AchievementMotivationService>();
builder.Services.AddScoped<IAchievementEstimationService, AchievementEstimationService>();
builder.Services.AddScoped<IAchievementRecommendationService, AchievementRecommendationService>();

// CORS Configuration
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

if (allowedOrigins.Length == 0 && builder.Environment.IsDevelopment())
{
    logger.LogWarning("⚠️ No CORS origins configured! Using defaults for development.");
    allowedOrigins =
    [
        "http://localhost:3000",
        "http://localhost:5173",
        "http://localhost:7255",
        "https://localhost:7255"
    ];
}

if (allowedOrigins.Length > 0)
{
    logger.LogInformation("🌐 CORS enabled for: {Origins}", string.Join(", ", allowedOrigins));

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policyBuilder =>
        {
            policyBuilder.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });
}
else
{
    if (builder.Environment.IsProduction())
    {
        logger.LogCritical("❌ No CORS origins configured for Production!");
        throw new Exception("CORS origins must be configured for Production.");
    }

    logger.LogWarning("⚠️ CORS is not configured, but continuing...");
}

// Фиксируем порт ТОЛЬКО для локальной разработки (не Docker)
if (builder.Environment.IsDevelopment() && !IsRunningInDocker())
{
    builder.WebHost.UseUrls("http://localhost:5000");
    logger.LogInformation("🔧 Port fixed to 5000 for local development");
}

var app = builder.Build();

// ВАЖНО: Применяем локализацию СРАЗУ после builder.Build()
var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(localizationOptions.Value);

if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

// Middleware
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Auto-migration and Seed
var autoMigrate = builder.Configuration.GetValue<bool>("AutoMigrate", false);

if (autoMigrate)
{
    logger.LogInformation("=== AUTO MIGRATION ENABLED ===");
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    logger.LogInformation("Running migrations...");
    db.Database.Migrate();
    logger.LogInformation("Migrations completed!");

    logger.LogInformation("Starting seed...");
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    await DbSeeder.SeedAsync(db, userManager);
    logger.LogInformation("Seed completed!");
}
else
{
    logger.LogInformation("=== AUTO MIGRATION DISABLED ===");
}

logger.LogInformation("🚀 Application started in {Environment} mode", app.Environment.EnvironmentName);

app.MapControllers();

// SignalR Hub
app.MapHub<NotificationHub>("/hubs/notifications");
logger.LogInformation("📡 SignalR Hub mapped to /hubs/notifications");

app.Run();

// Метод для определения Docker окружения
static bool IsRunningInDocker()
{
    return Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
}