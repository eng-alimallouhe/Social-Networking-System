using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Profiles.Profiles.Relations;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Infrastructure.Profiles.Profiles.Repositories;
using SNS.Infrastructure.Repositories.ProfileContext;

namespace SNS.Infrastructure.Profiles.Profiles;

public static class ProfilesInfrastructureDI
{
    public static IServiceCollection AddProfilesInfrastructureDI(this IServiceCollection services)
    {
        // Repositories
        
        services.AddScoped<ISoftDeletableRepository<Profile>, ProfileRepository>();
        services.AddScoped<ISoftDeletableRepository<ProfileView>, ProfileViewRepository>();
        services.AddScoped<IRepository<ReputationLedger>, ReputationLedgerRepository>();
        services.AddScoped<IRepository<SavedProfile>, SavedProfileRepository>();
        services.AddScoped<IRepository<ProfileSkill>, ProfileSkillRepository>();
        services.AddScoped<IRepository<ProfileTopic>, ProfileTopicRepository>();


        return services;
    }
}
