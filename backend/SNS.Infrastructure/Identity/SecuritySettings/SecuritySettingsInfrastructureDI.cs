using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.Identity.SecuritySettings.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;

namespace SNS.Infrastructure.Identity.SecuritySettings;

public static class SecuritySettingsInfrastructureDI
{
    public static IServiceCollection AddSecuritySettingsInfrastructureDI(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IRepository<RecoveryCode>, RecoveryCodeRepository>();
        services.AddScoped<IRepository<UserPasskey>, UserPasskeyRepository>();
        services.AddScoped<IRepository<UserSecuritySettings>, UserSecuritySettingsRepository>();
        
        return services;
    }
}
