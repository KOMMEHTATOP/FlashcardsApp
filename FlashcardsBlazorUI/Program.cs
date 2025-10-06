using FlashcardsBlazorUI.Components;
using FlashcardsBlazorUI.Services;
using FlashcardsBlazorUI.Helpers;
using FlashcardsBlazorUI.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using System.Text.Json.Serialization;
using CardService = FlashcardsBlazorUI.Services.CardService;
using GroupService = FlashcardsBlazorUI.Services.GroupService;
using StudySessionService = FlashcardsBlazorUI.Services.StudySessionService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents(options =>
    {
        options.DetailedErrors = builder.Environment.IsDevelopment();
    })
    .AddInteractiveServerComponents(options =>
    {
        options.DetailedErrors = builder.Environment.IsDevelopment();
        options.DisconnectedCircuitMaxRetained = 100;
        options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
        options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(1);
        options.MaxBufferedUnacknowledgedRenderBatches = 10;
    });

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<JwtAuthenticationStateProvider>());

builder.Services.AddSingleton<ITokenStorageService, TokenStorageService>();
builder.Services.AddTransient<AuthenticationHandler>();

var backendUrl = builder.Configuration["BackendUrl"] ?? "http://localhost:5000";
Console.WriteLine($"[FRONTEND] Using Backend URL: {backendUrl}");

builder.Services.AddHttpClient<IAuthService, AuthService>("AuthServiceClient", client =>
{
    client.BaseAddress = new Uri(backendUrl);
});

builder.Services.AddHttpClient("FlashcardsAPI", client =>
{
    client.BaseAddress = new Uri(backendUrl);
}).AddHttpMessageHandler<AuthenticationHandler>();

builder.Services.AddSingleton<IGroupNotificationService, GroupNotificationService>();
builder.Services.AddScoped<IGroupOrderService, GroupOrderService>();
builder.Services.AddScoped<GroupService>();
builder.Services.AddScoped<CardService>();
builder.Services.AddScoped<StudySessionService>();

builder.Services.AddScoped<GroupStore>(serviceProvider =>
{
    var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("FlashcardsAPI");
    var notificationService = serviceProvider.GetRequiredService<IGroupNotificationService>();
    
    return new GroupStore(httpClient, notificationService);
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

JSBridge.Initialize(app.Services);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// Отключаем HTTPS redirect в Production (Docker/Render)
if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

Console.WriteLine($"[FRONTEND] Application started in {app.Environment.EnvironmentName} mode");

app.Run();