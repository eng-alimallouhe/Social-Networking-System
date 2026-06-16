using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.Identity.SecuritySessions.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Infrastructure.Identity.SecuritySessions.Repositories;

namespace SNS.Infrastructure.Identity.SecuritySessions;

public static class SecuritySessionInfrastructureDI
{
    public static IServiceCollection AddSecuritySessionInfrastructureDI(this IServiceCollection services)
    {
        //Repositories
        services.AddScoped<IRepository<Device>, DeviceRepository>();
        services.AddScoped<IRepository<RefreshToken>, RefreshTokenRepository>();
        services.AddScoped<IRepository<SecuritySession>, SecuritySessionRepository>();


        return services;
    }
}
