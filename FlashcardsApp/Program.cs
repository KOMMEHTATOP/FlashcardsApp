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

builder.Services.AddOpenApi();

// Строка подключения к PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Регистрируем DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options
    => options.UseNpgsql(
        connectionString)); //регистрирует контекст базы данных как Scoped сервис (один экземпляр на HTTP запрос)

builder.Services.AddIdentity<User, IdentityRole<Guid>>() //регистрирует все сервисы Identity (там много скрытых)
    .AddEntityFrameworkStores<ApplicationDbContext>() // говорит Identity использовать EF Core для хранения данных
    .AddDefaultTokenProviders(); // регистрирует провайдеры токенов для сброса пароля, подтверждения email


// ------------------JWT------------------------ 
var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new Exception("JWT key is not configured. Please set Jwt:Key in configuration.");
}

var key = Encoding.ASCII.GetBytes(jwtKey);

Console.WriteLine($"JWT Key (first 10 chars): {jwtKey.Substring(0, 10)}...");
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


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(creator =>
    {
        creator.WithOrigins("https://localhost:7255", "http://localhost:5081")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); 
    });
});

var app = builder.Build();

// ---------------Тут начинается Middleware Pipeline---------------

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection(); //Перенаправляем HTTP запрос в HTTPS
app.UseCors();
app.UseAuthentication(); //Кто ты?
app.UseAuthorization(); // Что тебе можно?
app.MapControllers(); //настраивает маршрутизацию к контроллерам

app.Run();
