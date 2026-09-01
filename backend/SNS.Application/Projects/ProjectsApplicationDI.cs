using Microsoft.Extensions.DependencyInjection;
using SNS.Application.Projects.Abstractions;
using SNS.Application.Projects.Services;

namespace SNS.Application.Projects;

public static class ProjectsApplicationDI
{
    public static IServiceCollection AddProjectsApplicationDI(this IServiceCollection services)
    {
        services.AddScoped<IProjectCacheService, ProjectCacheService>();
        services.AddScoped<IProjectFeedService, ProjectFeedService>();
        
        return services;
    }
}
