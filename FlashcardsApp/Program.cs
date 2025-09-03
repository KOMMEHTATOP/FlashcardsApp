using FlashcardsApp.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Строка подключения к PostgreSQL
var connectionString = "Host=localhost;Port=5432;Database=FlashcardsDb;Username=postgres;Password=123";

// Регистрируем DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options=>options.UseNpgsql(connectionString));



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
