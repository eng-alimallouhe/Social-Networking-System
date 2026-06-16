using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SNS.Application.Shared.Settings;
using SNS.Infrastructure.Identity.Shared.Services;
using SNS.Infrastructure.Search.DI;
using SNS.Infrastructure.Shared.Services.Cashing;
using SNS.Infrastructure.Shared.Services.Identity;
using SNS.Infrastructure.Shared.Services.Loggings;
using SNS.Infrastructure.Shared.Services.Messaging;
using SNS.Infrastructure.Shared.Services.Storage;

namespace SNS.Infrastructure.Shared.Services;

public static class SharedServiceInfrastructureDI
{
    public static IServiceCollection AddSharedServiceInfrastructureDI(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddIdentityInfrastructureServices(configuration)
            .AddMessagingService(configuration)
            .AddIdentitySharedInfrastructureServices() 
            .AddLoggingServices()
            .AddSearchServices(configuration)
            .AddStorageServices(configuration)
            .AddCachingServices(configuration);

        return services;
    }
}
