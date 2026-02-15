using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.Abstractions.Repositories;
using SNS.Domain.Preferences.Entities;
using SNS.Infrastructure.Repositories.Preferences;

namespace SNS.Infrastructure.Repositories.Posts;

public static class PreferencesDI
{
    public static IServiceCollection AddPreferencesRepositories(
        this IServiceCollection services)
    {
        // Soft Delete
        services.AddScoped<
            ISoftDeletableRepository<Interest>, 
            InterestRepository>();
        
        services.AddScoped<
            ISoftDeletableRepository<InterestCategory>,
            InterestCategoryRepository>();
        
        services.AddScoped<
            ISoftDeletableRepository<Skill>, 
            SkillRepository>();

        services.AddScoped<
            ISoftDeletableRepository<SkillsCategory>, 
            SkillsCategoryRepository>();

        // Hard Delete
        services.AddScoped<
            IRepository<Tag>, 
            TagRepository>();
        
        services.AddScoped<
            IRepository<Topic>, 
            TopicRepository>();

        services.AddScoped<
            IRepository<InterestRequest>, 
            InterestRequestRepository>();

        services.AddScoped<
            IRepository<SkillRequest>, 
            SkillRequestRepository>();

        services.AddScoped<
            IRepository<TopicInterest>, 
            TopicInterestRepository>();


        return services;
    }
}