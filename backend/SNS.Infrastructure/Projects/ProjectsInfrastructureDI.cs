using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.Projects.Bridges;
using SNS.Domain.Projects.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Infrastructure.Repositories;
using SNS.Infrastructure.Repositories.Projects;

namespace SNS.Infrastructure.Projects;

public static class ProjectsInfrastructureDI
{
    public static IServiceCollection AddProjectsInfrastructureDI(this IServiceCollection services)
    {
        // Soft Delete
        services.AddScoped<ISoftDeletableRepository<Project>, ProjectRepository>();
        services.AddScoped<IRepository<ProjectMedia>, ProjectMediaRepository>();
        services.AddScoped<IRepository<ProjectMilestone>, ProjectMilestoneRepository>();
        services.AddScoped<IRepository<ProjectContributor>, ProjectContributorRepository>();
        services.AddScoped<IRepository<ProjectRating>, ProjectRatingRepository>();
        services.AddScoped<IRepository<ProjectSkill>, ProjectSkillRepository>();
        services.AddScoped<IRepository<ProjectTag>, ProjectTagRepository>();
        services.AddScoped<IRepository<SavedProject>, SavedProjectRepository>();
        services.AddScoped<ISoftDeletableRepository<ProjectView>, ProjectViewRepository>();

        return services;
    }
}
