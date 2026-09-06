using MaxMind.GeoIP2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SNS.Infrastructure.Identity.Shared.Services;
using SNS.Infrastructure.Search.DI;
using SNS.Infrastructure.Shared.BackgroundServices;
using SNS.Infrastructure.Shared.Services.AI;
using SNS.Infrastructure.Shared.Services.Cashing;
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
            .AddLoggingServices()
            .AddSearchServices(configuration)
            .AddStorageServices(configuration)
            .AddCachingServices(configuration)
            .AddBackgroundJobsServices()
            .AddAIDI(configuration);

        services.AddSingleton(sp =>
        {
            var basePath = AppContext.BaseDirectory;

            var dbPath = Path.Combine(basePath, "Shared", "Resources", "DataBases", "GeoLite2-City.mmdb");

            if (!File.Exists(dbPath))
            {
                var fallbackPath = Path.Combine(basePath, "GeoLite2-City.mmdb");
                if (File.Exists(fallbackPath))
                {
                    dbPath = fallbackPath;
                }
                else
                {
                    throw new FileNotFoundException($"GeoIP database not found at: {dbPath}");
                }
            }

            return new DatabaseReader(dbPath);
        });


        return services;
    }
}
