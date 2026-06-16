using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Infrastructure.Identity.ArchiveManagement.Jobs;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;
using SNS.Infrastructure.Shared.Services;

namespace SNS.Infrastructure.Shared;

public static class SharedInfrastructureDI
{
    public static IServiceCollection AddSharedInfrastructureDI(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddPersistenceDI(configuration);
        services.AddSharedServiceInfrastructureDI(configuration);

        services.AddQuartz(q =>
        {
            var jobKey = new JobKey("ArchiveCleanupJob", "IdentityGroups");
            q.AddJob<ArchiveCleanupJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("ArchiveCleanupJobTrigger", "IdentityGroups")
                .WithCronSchedule("0 0 3 * * ?"));
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }
}
