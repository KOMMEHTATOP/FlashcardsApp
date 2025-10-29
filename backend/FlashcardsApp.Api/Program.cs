using FlashcardsApp.Api.Extensions;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// LOGGING
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var logger = LoggerFactory.Create(config => config.AddConsole())
    .CreateLogger("Startup");

// ASP.NET CORE SERVICES
services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// INFRASTRUCTURE LAYER
services
    .AddDatabaseConfiguration(builder.Configuration, logger)
    .AddIdentityConfiguration()
    .AddJwtAuthentication(builder.Configuration, logger);

// APPLICATION LAYER
services
    .AddBusinessLogics()
    .AddConfigures(builder)
    .AddServices();

// CROSS-CUTTING CONCERNS
services
    .AddLocalizationConfiguration()
    .AddCorsConfiguration(builder.Configuration, builder.Environment, logger);

// OTHER
services.AddSignalRConfiguration(builder.Environment);

// DEVELOPMENT TOOLS
if (builder.Environment.IsDevelopment())
{
    services.AddSwaggerDocumentation();
    
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