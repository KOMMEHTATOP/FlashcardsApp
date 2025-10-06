using FlashcardsApp.Data;
using FlashcardsApp.Models;
using FlashcardsApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = null; 
    });

// Строка подключения к PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Регистрируем DbContext
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

Console.WriteLine($"[BACKEND] JWT Key configured (length: {jwtKey.Length})");

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
        
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"[JWT AUTH FAILED] {context.Exception.Message}");
                if (context.Exception.InnerException != null)
                    Console.WriteLine($"[JWT AUTH FAILED] Inner: {context.Exception.InnerException.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("[JWT] Токен валидирован успешно");
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                Console.WriteLine($"[JWT] Authorization header: {authHeader?.Substring(0, Math.Min(50, authHeader?.Length ?? 0))}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddScoped<GroupService>();
builder.Services.AddScoped<CardService>();
builder.Services.AddScoped<CardRatingService>();
builder.Services.AddScoped<StudySettingsService>();
builder.Services.AddScoped<StudySessionService>();

// CORS - читаем из переменной окружения
var allowedOrigins = builder.Configuration["ALLOWED_ORIGINS"]?.Split(',') 
    ?? new[] { "http://localhost:7255", "https://localhost:7255" };

Console.WriteLine($"[BACKEND] Allowed CORS origins: {string.Join(", ", allowedOrigins)}");

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

var app = builder.Build();

// Отключаем HTTPS redirect в Production (Docker/Render)
if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Применяем миграции ТОЛЬКО если явно указано (для Render.com)
var autoMigrate = builder.Configuration.GetValue<bool>("AutoMigrate", false);
if (autoMigrate)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    Console.WriteLine("[BACKEND] Auto-migration enabled. Applying migrations...");
    db.Database.Migrate();
    Console.WriteLine("[BACKEND] Migrations applied successfully");
}

Console.WriteLine($"[BACKEND] Application started in {app.Environment.EnvironmentName} mode");

app.Run();