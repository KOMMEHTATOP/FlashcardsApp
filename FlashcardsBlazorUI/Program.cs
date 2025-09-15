using FlashcardsBlazorUI.Components;
using FlashcardsBlazorUI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// HTTP склиент для обращения к моему API (нужно прописать мой порт).
builder.Services.AddHttpClient("FlashcardsAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5153/");
});

builder.Services.AddScoped<ApiService>();

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
