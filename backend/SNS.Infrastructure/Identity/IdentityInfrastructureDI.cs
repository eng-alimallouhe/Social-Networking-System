using Microsoft.Extensions.DependencyInjection;
using SNS.Infrastructure.Identity.ArchiveManagement;
using SNS.Infrastructure.Identity.Notifications;
using SNS.Infrastructure.Identity.SecuritySessions;
using SNS.Infrastructure.Identity.SecuritySettings;
using SNS.Infrastructure.Identity.Users;

namespace SNS.Infrastructure.Identity;


public static class IdentityInfrastructureDI
{
    public static IServiceCollection AddIdentityInfrastructureDI(
        this IServiceCollection services)
    {
        services.AddArchiveManagementInfrastructureDI();
        services.AddNotificationsInfrastructureDI();
        services.AddSecuritySessionInfrastructureDI();
        services.AddSecuritySettingsInfrastructureDI();
        services.AddUsersInfrastructureDI();

        services.AddSignalR();

        return services;
    }
}
