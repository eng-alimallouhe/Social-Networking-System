using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.Jobs.Entities;
using SNS.Domain.Jobs.Relations;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Infrastructure.Repositories.Jobs;

namespace SNS.Infrastructure.Jobs;

public static class JobsInfrastructureDI
{
    public static IServiceCollection AddJobsInfrastructureDI(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<ISoftDeletableRepository<Job>, JobRepository>();
        services.AddScoped<ISoftDeletableRepository<JobApplication>, JobApplicationRepository>();
        services.AddScoped<ISoftDeletableRepository<Company>, CompanyRepository>();
        services.AddScoped<IRepository<JobSkill>, JobSkillRepository>();
        services.AddScoped<IRepository<CompanyAdministrator>, CompanyAdministratorRepository>();
        services.AddScoped<IRepository<SavedJob>, SavedJobRepository>();
        services.AddScoped<IRepository<CompanyCreateRequest>, CompanyCreateRequestRepository>();

        return services;
    }
}
