using Microsoft.Extensions.DependencyInjection;
using SNS.Application.Shared.Abstractions.BackgroundJobs;
using SNS.Domain.Identity.ArchiveManagement.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Infrastructure.Identity.ArchiveManagement.Repositories;
using SNS.Infrastructure.Shared.BackgroundJobs;

namespace SNS.Infrastructure.Identity.ArchiveManagement;

public static class ArchiveManagementInfrastructureDI
{
    public static IServiceCollection AddArchiveManagementInfrastructureDI(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IRepository<IdentityArchive>, IdentityArchiveRepository>();
        services.AddScoped<IRepository<PasswordArchive>, PasswordArchiveRepository>();
        services.AddScoped<IRepository<UserArchive>, UserArchiveRepository>();
        services.AddScoped<IRepository<ExportDataRequest>, ExportDataRequestRepository>();


        services.AddScoped<IJobSchedulerService, QuartzJobSchedulerService>();

        return services;
    }
}
