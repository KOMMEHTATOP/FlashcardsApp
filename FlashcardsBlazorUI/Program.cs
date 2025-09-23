using FlashcardsAppContracts.DTOs.Responses;
using FlashcardsBlazorUI.Components;
using FlashcardsBlazorUI.Services;
using FlashcardsBlazorUI.Helpers;
using FlashcardsBlazorUI.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Razor/Blazor
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

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// ===== Аутентификация =====
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<JwtAuthenticationStateProvider>());

builder.Services.AddSingleton<ITokenStorageService, TokenStorageService>();
builder.Services.AddTransient<AuthenticationHandler>();

// HttpClient для AuthService (без AuthenticationHandler)
builder.Services.AddHttpClient<IAuthService, AuthService>("AuthServiceClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5153/");
});

// HttpClient для остальных API с токенами
builder.Services.AddHttpClient("FlashcardsAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5153/");
}).AddHttpMessageHandler<AuthenticationHandler>();

// ===== Бизнес-сервисы =====
// ✅ Singleton для уведомлений - должен быть первым
builder.Services.AddSingleton<IGroupNotificationService, GroupNotificationService>();

builder.Services.AddScoped<IGroupOrderService, GroupOrderService>();
builder.Services.AddScoped<GroupService>();

// ✅ GroupStore теперь зависит от IGroupNotificationService, поэтому регистрируем его после
builder.Services.AddScoped<GroupStore>(serviceProvider =>
{
    var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("FlashcardsAPI");
    var notificationService = serviceProvider.GetRequiredService<IGroupNotificationService>();
    
    return new GroupStore(httpClient, notificationService);
});

builder.Services.AddScoped<CardService>();

// ===== CORS =====
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7255", "http://localhost:5081")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ===== Авторизация для Blazor =====
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

JSBridge.Initialize(app.Services);

// ===== Middleware =====
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors();

// Для Blazor Server с кастомным AuthenticationStateProvider
// UseAuthentication/UseAuthorization не нужны

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();