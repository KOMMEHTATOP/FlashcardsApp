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

builder.Services.AddOpenApi();

// Строка подключения к PostgreSQL
var connectionString = "Host=localhost;Port=5432;Database=FlashcardsDb;Username=postgres;Password=123";

// Регистрируем DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options
    => options.UseNpgsql(
        connectionString)); //регистрирует контекст базы данных как Scoped сервис (один экземпляр на HTTP запрос)

builder.Services.AddIdentity<User, IdentityRole<Guid>>() //регистрирует все сервисы Identity (там много скрытых)
    .AddEntityFrameworkStores<ApplicationDbContext>() // говорит Identity использовать EF Core для хранения данных
    .AddDefaultTokenProviders(); // регистрирует провайдеры токенов для сброса пароля, подтверждения email

// ------------------JWT------------------------ 
var jwtKey = "your-super-secret-key-at-least-32-characters-long!";
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme; //говорит системе использовать JWT вместо cookies
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; //Отключает требование HTTPS для JWT metadata.
        //В production должно быть true! Сейчас false, потому что работаешь с localhost по HTTP
        options.SaveToken = true; //Сохраняет JWT токен в HttpContext после валидации.
        //Позволяет получить токен внутри контроллера через HttpContext.GetTokenAsync("access_token").
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key), //Проверяет подпись токена.
            //Гарантирует, что токен создан именно твоим сервером, а не подделан
            ValidateIssuer = false,
            ValidateAudience = false, //Отключает проверку издателя (кто создал токен)
            //и аудитории (для кого предназначен). Для простого API можно отключить.

            ValidateLifetime =
                true, //Проверяет, не истек ли срок действия токена. Автоматически отклоняет просроченные токены.

            ClockSkew = TimeSpan.Zero //Убирает временную погрешность при проверке срока действия.
            //По умолчанию ASP.NET Core добавляет 5 минут "запаса" - эта настройка убирает его.
        };
    });


builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
}); //регистрирует сервисы для работы с контроллерами MVC
builder.Services.AddScoped<GroupService>();
builder.Services.AddScoped<CardService>();
builder.Services.AddScoped<CardRatingService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("https://localhost:7255", "http://localhost:5081")
            .AllowAnyHeader()
            .AllowAnyMethod();
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
