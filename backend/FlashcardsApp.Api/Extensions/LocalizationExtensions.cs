using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace FlashcardsApp.Api.Extensions;

public static class LocalizationExtensions
{
    public static IServiceCollection AddLocalizationConfiguration(
        this IServiceCollection services)
    {
        services.AddLocalization();

        var supportedCultures = new[]
        {
            new CultureInfo("ru"),
            new CultureInfo("en")
        };

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture("ru");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
            options.FallBackToParentUICultures = true;
        });

        return services;
    }
}
