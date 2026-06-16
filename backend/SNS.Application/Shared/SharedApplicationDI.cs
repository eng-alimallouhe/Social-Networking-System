using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SNS.Application.Shared.Settings;

namespace SNS.Application.Shared;

public static class SharedApplicationDI
{
    public static IServiceCollection AddSharedApplicationDI(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSettingsDI(configuration);
        services.AddSharedApplicationDI(configuration);

        return services;
    }
}
