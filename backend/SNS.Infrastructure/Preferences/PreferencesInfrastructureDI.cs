using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.Preferences.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Infrastructure.Preferences.Repositories;

namespace SNS.Infrastructure.Preferences;

public static class PreferencesInfrastructureDI
{
    public static IServiceCollection AddPreferencesInfrastructureDI(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<ISoftDeletableRepository<Skill>, SkillRepository>();
        services.AddScoped<ISoftDeletableRepository<SkillsCategory>, SkillsCategoryRepository>();
        services.AddScoped<IRepository<Tag>, TagRepository>();
        services.AddScoped<IRepository<Topic>, TopicRepository>();


        return services;
    }
}
