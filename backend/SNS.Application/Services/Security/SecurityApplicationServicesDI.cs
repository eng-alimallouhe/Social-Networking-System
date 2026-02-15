using Microsoft.Extensions.DependencyInjection;
using SNS.Application.Abstractions.Security;

namespace SNS.Application.Services.Security;

public static class SecurityApplicationServicesDI
{
    public static IServiceCollection AddSecurityApplicationServices(
        this IServiceCollection services)
    {
        services.AddScoped<
            IArchiveService, ArchiveService>();

        services.AddScoped<
            IPendingUpdatesService, PendingUpdatesService>();

        return services;
    }
}
