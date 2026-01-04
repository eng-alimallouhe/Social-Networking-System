using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.Abstractions.Repositories;
using SNS.Infrastructure.Repositories.BaseRepositories;
using SNS.Infrastructure.Repositories.Communities;
using SNS.Infrastructure.Repositories.Education;
using SNS.Infrastructure.Repositories.Jobs;
using SNS.Infrastructure.Repositories.Posts;
using SNS.Infrastructure.Repositories.ProfileContext;
using SNS.Infrastructure.Repositories.Projects;
using SNS.Infrastructure.Repositories.QA;
using SNS.Infrastructure.Repositories.Resumes;
using SNS.Infrastructure.Repositories.Security;
using SNS.Infrastructure.Repositories.SocialGraph;

namespace SNS.Infrastructure.Repositories;

public static class RepositoriesDI
{
    public static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {

        services
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddSecurityRepositories()
            .AddSocialGraphRepositories()
            .AddCommunitiesRepositories()
            .AddEducationRepositories()
            .AddJobsRepositories()
            .AddPostsRepositories()
            .AddPreferencesRepositories()
            .AddProfileContextRepositories()
            .AddProjectsRepositories()
            .AddQARepositories()
            .AddResumesRepositories();
    
        return services;
    }
}
