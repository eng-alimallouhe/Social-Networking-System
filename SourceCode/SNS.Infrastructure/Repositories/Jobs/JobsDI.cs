using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.Abstractions.Repositories;
using SNS.Domain.Jobs.Entities;

namespace SNS.Infrastructure.Repositories.Jobs;

public static class JobsDI
{
    public static IServiceCollection AddJobsRepositories(
        this IServiceCollection services)
    {
        // Soft Delete
        services.AddScoped<ISoftDeletableRepository<Job>, JobRepository>();
        services.AddScoped<ISoftDeletableRepository<JobApplication>, JobApplicationRepository>();

        // Hard Delete
        services.AddScoped<IRepository<JobSkill>, JobSkillRepository>();

        return services;
    }
}