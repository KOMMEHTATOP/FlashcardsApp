using FlashcardsApp.Data;
using FlashcardsApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Строка подключения к PostgreSQL
var connectionString = "Host=localhost;Port=5432;Database=FlashcardsDb;Username=postgres;Password=123";

// Регистрируем DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString)); //регистрирует контекст базы данных как Scoped сервис (один экземпляр на HTTP запрос)

builder.Services.AddIdentity<User, IdentityRole<Guid>>() //регистрирует все сервисы Identity (там много скрытых)
    .AddEntityFrameworkStores<ApplicationDbContext>() // говорит Identity использовать EF Core для хранения данных
    .AddDefaultTokenProviders(); // регистрирует провайдеры токенов для сброса пароля, подтверждения email

builder.Services.AddControllers(); //регистрирует сервисы для работы с контроллерами MVC

var app = builder.Build();

// ---------------Тут начинается Middleware Pipeline---------------

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection(); //Перенаправляем HTTP запрос в HTTPS
app.UseAuthentication(); //Кто ты?
app.UseAuthorization(); // Что тебе можно?
app.MapControllers(); //настраивает маршрутизацию к контроллерам

app.Run();
