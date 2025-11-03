using FlashcardsApp.Models.Constants;

namespace FlashcardsApp.Api.Extensions;

public static class ConfigExtentions
{
    public static IServiceCollection AddConfigures(
        this IServiceCollection services,
        WebApplicationBuilder builder)
    {
        services.Configure<RewardSettings>(builder.Configuration.GetSection("RewardSettings"));
        return services;
    }

}
