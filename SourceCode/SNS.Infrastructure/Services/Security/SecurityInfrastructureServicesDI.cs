using MaxMind.GeoIP2;
using Microsoft.Extensions.DependencyInjection;
using SNS.Application.Abstractions.Security;

namespace SNS.Infrastructure.Services.Security;

public static class SecurityInfrastructureServicesDI
{
    public static IServiceCollection AddSecurityInfrastructureServices(
        this IServiceCollection services)
    {
        services.AddScoped<
            IGeoLocationService, GeoLocationService>();
        
        services.AddScoped<
            ISessionService, SessionService>();
        
        services.AddScoped<
            IUserSessionService, UserSessionService>();

        services.AddSingleton(sp =>
        {
            var basePath = AppContext.BaseDirectory;

            var dbPath = Path.Combine(basePath, "Resources", "DataBases", "GeoLite2-Country.mmdb");

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
