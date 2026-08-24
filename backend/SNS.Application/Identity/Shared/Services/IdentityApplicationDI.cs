using Fido2NetLib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SNS.Application.Identity.ArchiveManagement.Abstractions;
using SNS.Application.Identity.ArchiveManagement.Services;
using SNS.Application.Identity.Notifications.Abstractions;
using SNS.Application.Identity.Notifications.Services;
using SNS.Application.Identity.SecuritySessions.Shared.Abstractions;
using SNS.Application.Identity.SecuritySessions.Shared.Services;
using SNS.Application.Identity.Shared.Abstractions;

namespace SNS.Application.Identity.Shared.Services;

public static class IdentityApplicationDI
{
    public static IServiceCollection AddIdentityApplicationServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services
            .AddScoped<ISessionService, SessionService>();

        services.AddScoped<
            IArchiveService, ArchiveService>();

        services.AddScoped<
            IDeviceService, DeviceService>();

        services.AddScoped<
            IPendingUpdatesService, PendingUpdatesService>();

        services.AddScoped<
            ICodeService, CodeService>();


        services.AddScoped<
            IUserCacheService, UserCacheService>();

        services.AddScoped<
            IUrlGeneratorService, UrlGeneratoreService>();

        services.Configure<Fido2Configuration>(configuration.GetSection("Fido2"));

        services.AddScoped<IFido2, Fido2>(provider =>
        {
            var fidoOptions = provider.GetRequiredService<IOptions<Fido2Configuration>>().Value;
            return new Fido2(new Fido2Configuration
            {
                ServerDomain = fidoOptions.ServerDomain,
                ServerName = fidoOptions.ServerName,
                ServerIcon = fidoOptions.ServerIcon,
                Origins = fidoOptions.Origins,
                TimestampDriftTolerance = fidoOptions.TimestampDriftTolerance <= 0 ? 300000 : fidoOptions.TimestampDriftTolerance
            });
        });

        services.AddSingleton<IOnlineUserTracker, OnlineUserTracker>();
        services.AddScoped<IAuthenticationFlowService, AuthenticationFlowService>();

        services.AddScoped<INotificationDeliveryService, NotificationDeliveryService>();
        services.AddScoped<INotificationLocalizerService, NotificationLocalizerService>();

        return services;
    }
}
