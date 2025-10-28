using FlashcardsApp.Api.Extensions;
using FlashcardsApp.Models.Constants;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Логирование
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var logger = LoggerFactory.Create(config => { config.AddConsole(); })
    .CreateLogger("Startup");

// Контроллеры с JSON настройками
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// ===== ВСЕ НАСТРОЙКИ ЧЕРЕЗ EXTENSION METHODS =====

// База данных (внутри: connectionString + DbContext)
builder.Services.AddDatabaseConfiguration(builder.Configuration, logger);

// Локализация (внутри: AddLocalization + RequestLocalizationOptions)
builder.Services.AddLocalizationConfiguration();

// Identity с локализацией
builder.Services.AddIdentityConfiguration();

// JWT (внутри: проверка ключа + все настройки authentication)
builder.Services.AddJwtAuthentication(builder.Configuration, logger);

// SignalR
builder.Services.AddSignalRConfiguration(builder.Environment);

// Swagger
builder.Services.AddSwaggerDocumentation();

// CORS (внутри: чтение конфига + allowedOrigins + настройка)
builder.Services.AddCorsConfiguration(builder.Configuration, builder.Environment, logger);

// Настройки наград
builder.Services.Configure<RewardSettings>(
    builder.Configuration.GetSection("RewardSettings"));

// Регистрация ВСЕХ сервисов приложения
builder.Services.AddApplicationServices();

// Порт для локальной разработки
if (builder.Environment.IsDevelopment() && !IsRunningInDocker())
{
    builder.WebHost.UseUrls("http://localhost:5000");
    logger.LogInformation("🔧 Port fixed to 5000 for local development");
}

var app = builder.Build();

// ===== MIDDLEWARE PIPELINE + МИГРАЦИИ + МАППИНГ =====
app.ConfigureMiddleware(builder.Configuration, logger, builder.Environment);

app.Run();

// Вспомогательный метод
static bool IsRunningInDocker()
{
    return Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
}