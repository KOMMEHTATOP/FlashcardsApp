using FlashcardsBlazorUI.Components;
using FlashcardsBlazorUI.Services;

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

// HTTP склиент для обращения к моему API 
builder.Services.AddHttpClient("FlashcardsAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5153/"); 
}).AddHttpMessageHandler<AuthenticationHandler>();


builder.Services.AddSingleton<ITokenManager, TokenManager>();
builder.Services.AddTransient<AuthenticationHandler>();

builder.Services.AddScoped<IGroupOrderService, GroupOrderService>();

builder.Services.AddScoped<GroupService>();
builder.Services.AddScoped<CardService>();
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
