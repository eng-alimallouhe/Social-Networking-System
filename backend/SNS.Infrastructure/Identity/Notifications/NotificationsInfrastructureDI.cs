using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using SNS.Application.Identity.Notifications.Abstractions;
using SNS.Domain.Identity.Notifications.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Infrastructure.Identity.Notifications.Hubs;
using SNS.Infrastructure.Identity.Notifications.Services;

namespace SNS.Infrastructure.Identity.Notifications;

public static class NotificationsInfrastructureDI
{
    public static IServiceCollection AddNotificationsInfrastructureDI(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IRepository<Notification>, NotificationRepository>();
        services.AddScoped<IRepository<UserNotificationPreferences>, UserNotificationPreferencesRepository>();

        services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
        services.AddScoped<IInAppNotificationService, InAppNotificationService>();
        services.AddScoped<IPushNotificationService, PushNotificationService>();
        
        return services;
    }
}
