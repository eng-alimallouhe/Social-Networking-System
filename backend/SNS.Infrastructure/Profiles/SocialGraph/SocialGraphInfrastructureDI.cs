using Microsoft.Extensions.DependencyInjection;
using SNS.Application.Profiles.SocialGraph.Abstractions;
using SNS.Application.Profiles.SocialGraph.Services;
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

        // Service DI:
        services.AddScoped<ISocialPolicyService, SocialPolicyService>();

        return services;
    }
}
