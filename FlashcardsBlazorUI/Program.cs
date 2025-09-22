using FlashcardsBlazorUI.Components;
using FlashcardsBlazorUI.Services;
using FlashcardsBlazorUI.Helpers;
using FlashcardsBlazorUI.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents(options =>
    {
        options.DetailedErrors = true; // только для разработки
    })
    .AddInteractiveServerComponents(options =>
    {
        options.DetailedErrors = true;
        options.DisconnectedCircuitMaxRetained = 100;
        options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
        options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(1);
        options.MaxBufferedUnacknowledgedRenderBatches = 10;
    });

// JWT Authentication - регистрируем наш AuthenticationStateProvider
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => 
    provider.GetRequiredService<JwtAuthenticationStateProvider>());

// Регистрируем TokenStorageService как SINGLETON (чтобы один экземпляр на всё приложение)
builder.Services.AddSingleton<ITokenStorageService, TokenStorageService>();

// Регистрируем AuthenticationHandler
builder.Services.AddTransient<AuthenticationHandler>();

// HTTP клиент ДЛЯ AuthService БЕЗ AuthenticationHandler (избегаем циклическую зависимость)
builder.Services.AddHttpClient<IAuthService, AuthService>("AuthServiceClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5153/");
});

// HTTP клиент для ОСТАЛЬНЫХ сервисов С AuthenticationHandler
builder.Services.AddHttpClient("FlashcardsAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5153/"); 
}).AddHttpMessageHandler<AuthenticationHandler>();

// УБРАНО: builder.Services.AddScoped<IAuthService, AuthService>(); - дублирование!

// Ваши бизнес-сервисы
builder.Services.AddSingleton<IGroupNotificationService, GroupNotificationService>();
builder.Services.AddScoped<IGroupOrderService, GroupOrderService>();
builder.Services.AddScoped<GroupService>();
builder.Services.AddScoped<CardService>();

// CORS настройки
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("https://localhost:7255", "http://localhost:5081")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Базовые сервисы для авторизации
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

JSBridge.Initialize(app.Services);

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors();

// ВАЖНО: Убрали UseAuthentication() и UseAuthorization() 
// Для Blazor Server с кастомным AuthenticationStateProvider они не нужны

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();