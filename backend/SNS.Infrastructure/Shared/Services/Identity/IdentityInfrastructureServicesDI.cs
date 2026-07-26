using MaxMind.GeoIP2;
using Microsoft.Extensions.DependencyInjection;

namespace SNS.Infrastructure.Shared.Services.Identity;

public static class IdentitySharedInfrastructureServicesDI
{
    public static IServiceCollection AddIdentitySharedInfrastructureServices(
        this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var basePath = AppContext.BaseDirectory;

            var dbPath = Path.Combine(basePath , "Shared", "Resources", "DataBases", "GeoLite2-Country.mmdb");

            if (!File.Exists(dbPath))
            {
                var fallbackPath = Path.Combine(basePath, "GeoLite2-Country.mmdb");
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
