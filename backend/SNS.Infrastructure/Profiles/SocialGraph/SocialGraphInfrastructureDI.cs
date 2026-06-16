using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.Profiles.SocialGraph.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Infrastructure.Profiles.SocialGraph.Repositories;

namespace SNS.Infrastructure.Profiles.SocialGraph;

public static class SocialGraphInfrastructureDI
{
    public static IServiceCollection AddSocialGraphInfrastructureDI(this IServiceCollection services)
    {
        services.AddScoped<IRepository<Block>, BlockRepository>();
        services.AddScoped<IRepository<Follow>, FollowRepository>();
        services.AddScoped<IRepository<Mute>,MuteRepository>();

        return services;
    }
}
