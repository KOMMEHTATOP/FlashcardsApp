using FlashcardsApp.Api.Extensions;
using FlashcardsApp.Models.Constants;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// LOGGING
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var logger = LoggerFactory.Create(config => config.AddConsole())
    .CreateLogger("Startup");

// ASP.NET CORE SERVICES
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// INFRASTRUCTURE LAYER
builder.Services.AddDatabaseConfiguration(builder.Configuration, logger);
builder.Services.AddIdentityConfiguration();
builder.Services.AddJwtAuthentication(builder.Configuration, logger);

// APPLICATION LAYER
builder.Services.AddApplicationServices();

builder.Services.Configure<RewardSettings>(
    builder.Configuration.GetSection("RewardSettings"));

// CROSS-CUTTING CONCERNS
builder.Services.AddLocalizationConfiguration();
builder.Services.AddCorsConfiguration(builder.Configuration, builder.Environment, logger);

// FEATURES
builder.Services.AddSignalRConfiguration(builder.Environment);

// DEVELOPMENT TOOLS
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerDocumentation();
    
    if (!IsRunningInDocker())
    {
        builder.WebHost.UseUrls("http://localhost:5000");
        logger.LogInformation("🔧 Port fixed to 5000 for local development");
    }
}

// BUILD & CONFIGURE PIPELINE
var app = builder.Build();

app.ConfigureMiddleware(builder.Configuration, logger, builder.Environment);

app.Run();

// HELPERS
static bool IsRunningInDocker()
{
    return Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
}