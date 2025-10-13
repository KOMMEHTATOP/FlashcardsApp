using FlashcardsApp.Data;
using FlashcardsApp.Models;
using FlashcardsApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = null; 
    });

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
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
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
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

// Регистрация сервисов
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<GroupService>();
builder.Services.AddScoped<CardService>();
builder.Services.AddScoped<CardRatingService>();
builder.Services.AddScoped<StudySettingsService>();
builder.Services.AddScoped<StudySessionService>();
builder.Services.AddScoped<UserStatisticsService>();
builder.Services.AddScoped<AchievementService>(); 
builder.Services.AddScoped<GamificationService>();
builder.Services.AddScoped<StudyService>();

// CORS Configuration
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

// Если конфигурация пустая - берем из переменных окружения (для Docker)
if (allowedOrigins.Length == 0)
{
    var envOrigins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS");
    if (!string.IsNullOrEmpty(envOrigins))
    {
        allowedOrigins = envOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(o => o.Trim())
            .ToArray();
        Console.WriteLine("📦 Using CORS origins from environment variable");
    }
}

// Fallback для локальной разработки
if (allowedOrigins.Length == 0 && builder.Environment.IsDevelopment())
{
    Console.WriteLine("⚠️  WARNING: No CORS origins configured! Using defaults for development.");
    allowedOrigins = new[] 
    { 
        "http://localhost:3000",
        "http://localhost:5173",
        "http://localhost:7255",
        "https://localhost:7255"
    };
}

if (allowedOrigins.Length > 0)
{
    Console.WriteLine($"🌐 CORS enabled for: {string.Join(", ", allowedOrigins)}");
    
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
    Console.WriteLine("❌ ERROR: No CORS origins configured!");
    throw new Exception("CORS origins must be configured for security reasons.");
}

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

// ВАЖНО: Порядок middleware
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

// Auto-migration and Seed
var autoMigrate = builder.Configuration.GetValue<bool>("AutoMigrate", false);
if (autoMigrate)
{
    Console.WriteLine("=== AUTO MIGRATION ENABLED ===");
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    Console.WriteLine("Running migrations...");
    db.Database.Migrate();
    Console.WriteLine("Migrations completed!");
    
    // Seed данные
    Console.WriteLine("Starting seed...");
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    await DbSeeder.SeedAsync(db, userManager);
    Console.WriteLine("Seed completed!");
}
else
{
    Console.WriteLine("=== AUTO MIGRATION DISABLED ===");
}

Console.WriteLine($"🚀 Application started in {app.Environment.EnvironmentName} mode");

app.Run();