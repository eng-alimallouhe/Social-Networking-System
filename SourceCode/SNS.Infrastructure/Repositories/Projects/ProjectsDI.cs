using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.Abstractions.Repositories;
using SNS.Domain.Projects.Bridges;
using SNS.Domain.Projects.Entities;

namespace SNS.Infrastructure.Repositories.Projects;

public static class ProjectsDI
{
    public static IServiceCollection AddProjectsRepositories(
        this IServiceCollection services)
    {
        // Soft Delete
        services.AddScoped<
            ISoftDeletableRepository<Project>, 
            ProjectRepository>();

        // Hard Delete
        services.AddScoped<
            IRepository<ProjectMedia>, 
            ProjectMediaRepository>();

        services.AddScoped<
            IRepository<ProjectMilestone>, 
            ProjectMilestoneRepository>();

        services.AddScoped<
            IRepository<ProjectContributor>, 
            ProjectContributorRepository>();

        services.AddScoped<
            IRepository<ProjectRating>, 
            ProjectRatingRepository>();

        services.AddScoped<
            IRepository<ProjectSkill>, 
            ProjectSkillRepository>();

        services.AddScoped<
            IRepository<ProjectTag>, 
            ProjectTagRepository>();

        services.AddScoped<
            IRepository<ProjectView>, 
            ProjectViewRepository>();

        return services;
    }
}