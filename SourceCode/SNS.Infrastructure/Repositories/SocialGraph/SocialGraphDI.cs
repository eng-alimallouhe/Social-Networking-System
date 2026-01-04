using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.Abstractions.Repositories;
using SNS.Domain.SocialGraph;
using SNS.Domain.SocialGraph.Bridges;

namespace SNS.Infrastructure.Repositories.SocialGraph;

public static class SocialGraphDI
{
    public static IServiceCollection AddSocialGraphRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<
            ISoftDeletableRepository<Profile>, 
            ProfileRepository>();

        services.AddScoped<
            IRepository<Follow>, 
            FollowRepository>();
        
        services.AddScoped<
            IRepository<Block>, 
            BlockRepository>();

        return services;
    }
}