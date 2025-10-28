using FlashcardsApp.Api.Resources;
using FlashcardsApp.DAL;
using FlashcardsApp.DAL.Models;
using Microsoft.AspNetCore.Identity;

namespace FlashcardsApp.Api.Extensions;

public static class IdentityExtensions
{
    public static IServiceCollection AddIdentityConfiguration(
        this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddErrorDescriber<LocalizedIdentityErrorDescriber>()
            .AddDefaultTokenProviders();

        return services;
    }
}
