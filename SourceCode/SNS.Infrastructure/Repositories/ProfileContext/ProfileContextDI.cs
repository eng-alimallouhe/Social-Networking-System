using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.Abstractions.Repositories;
using SNS.Domain.ProfileContext.Bridges;

namespace SNS.Infrastructure.Repositories.ProfileContext;

public static class ProfileContextDI
{
    public static IServiceCollection AddProfileContextRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<
            IRepository<ProfileInterest>, 
            ProfileInterestRepository>();

        services.AddScoped<
            IRepository<ProfileInterestRequest>, 
            ProfileInterestRequestRepository>();

        services.AddScoped<
            IRepository<ProfileSkill>, 
            ProfileSkillRepository>();

        services.AddScoped<
            IRepository<ProfileSkillRequest>, 
            ProfileSkillRequestRepository>();

        services.AddScoped<
            IRepository<ProfileTopic>, 
            ProfileTopicRepository>();


        return services;
    }
}