using Microsoft.Extensions.DependencyInjection;

namespace SNS.Infrastructure.Shared.BackgroundServices;

public static class BackgroundJobsServicersDI
{
    public static IServiceCollection AddBackgroundJobsServices(
        this IServiceCollection services)
    {
        services.AddHostedService<ProcessOutboxMessagesJob>();

        return services;
    }
}