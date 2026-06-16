using Microsoft.Extensions.DependencyInjection;
using SNS.Application.Abstractions.Loggings;

namespace SNS.Infrastructure.Shared.Services.Loggings;

public static class LoggingServicesDI
{
    public static IServiceCollection AddLoggingServices(
        this IServiceCollection services)
    {
        services.AddScoped(typeof(IAppLogger<>), typeof(AppLogger<>));

        return services; 
    }
}
