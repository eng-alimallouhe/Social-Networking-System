using Microsoft.Extensions.DependencyInjection;
using SNS.Application.Profiles.Profiles.abstractions;
using SNS.Application.Profiles.Profiles.Services;

namespace SNS.Application.Profiles.Profiles;

public static class ApplicationSocialGraphDI
{
    public static IServiceCollection AddApplicationSocialGrcontentaph(this IServiceCollection services)
    {
        services.AddScoped<IProfileCacheService, ProfileCacheService>();
        return services;
    }
}
