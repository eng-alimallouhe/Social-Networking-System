using Microsoft.Extensions.DependencyInjection;
using SNS.Application.Shared.Abstractions.BackgroundJobs;

namespace SNS.Infrastructure.Shared.BackgroundJobs;

public static class BackgroundJobsServicersDI
{
    public static IServiceCollection AddBackgroundJobsServices(
        this IServiceCollection services)
    {
        services.AddHostedService<
            ProcessOutboxMessagesJob>();

        services.AddTransient<IBackgroundJobService, BackgroundJobService>();

        return services;
    }
}
