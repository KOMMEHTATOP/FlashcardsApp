using FlashcardsApp.Configuration;
using FlashcardsApp.Data;
using FlashcardsApp.Interfaces;
using FlashcardsApp.Interfaces.Achievements;
using FlashcardsApp.Models;
using FlashcardsApp.Services;
using FlashcardsApp.Services.Achievements;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Добавляем логирование
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var logger = LoggerFactory.Create(config =>
{
    config.AddConsole();
}).CreateLogger("Startup");

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

builder.Services.AddDbContext<ApplicationDbContext>(options
    => options.UseNpgsql(connectionString));

builder.Services.AddIdentity<User, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
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
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<CardService>();
builder.Services.AddScoped<CardRatingService>();
builder.Services.AddScoped<StudySettingsService>();
builder.Services.AddScoped<StudySessionService>();
builder.Services.AddScoped<UserStatisticsService>();

// Gamification
builder.Services.AddScoped<IGamificationService, GamificationService>();
builder.Services.AddScoped<IStudyService, StudyService>();

// Achievement services
builder.Services.AddScoped<IAchievementService, AchievementService>();
builder.Services.AddScoped<IAchievementRewardService, AchievementRewardService>();
builder.Services.AddScoped<IAchievementLeaderboardService, AchievementLeaderboardService>();

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
app.Run();

// Метод для определения Docker окружения
static bool IsRunningInDocker()
{
    return Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
}
