using Microsoft.Extensions.DependencyInjection;
using Quartz;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Infrastructure.Identity.ArchiveManagement.Jobs;
using SNS.Infrastructure.Identity.Users.BackgroundJobs;
using SNS.Infrastructure.Identity.Users.Repositories;

namespace SNS.Infrastructure.Identity.Users;

public static class UsersInfrastructureDI
{
    public static IServiceCollection AddUsersInfrastructureDI(this IServiceCollection services)
    {
        services.AddScoped<IRepository<User>, UserRepository>();
        services.AddScoped<ISoftDeletableRepository<Role>, RoleRepository>();

        services.AddQuartz(q =>
        {
            var archiveJobKey = new JobKey("ArchiveCleanupJob", "IdentityGroups");

            q.AddJob<ArchiveCleanupJob>(opts => opts.WithIdentity(archiveJobKey));

            q.AddTrigger(opts => opts
                .ForJob(archiveJobKey)
                .WithIdentity("ArchiveCleanupJobTrigger", "IdentityGroups")
                .WithCronSchedule("0 0 3 * * ?"));

            var hardDeleteJobKey = new JobKey("UserHardDeletionJob", "IdentityGroups");

            q.AddJob<UserHardDeletionJob>(opts => opts.WithIdentity(hardDeleteJobKey));

            q.AddTrigger(opts => opts
                .ForJob(hardDeleteJobKey)
                .WithIdentity("UserHardDeletionJobTrigger", "IdentityGroups")
                .WithCronSchedule("0 30 3 * * ?"));
        });


        return services;
    }
}
