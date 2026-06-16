using Microsoft.Extensions.DependencyInjection;
using SNS.Infrastructure.Profiles.Profiles;
using SNS.Infrastructure.Profiles.SocialGraph;

namespace SNS.Infrastructure.Profiles;

public static class ProfileContextInfrastructureDI
{
    public static IServiceCollection AddProfileContextInfrastructureDI(this IServiceCollection services)
    {
        services.AddProfilesInfrastructureDI();
        services.AddSocialGraphInfrastructureDI();

        return services;
    }
}
