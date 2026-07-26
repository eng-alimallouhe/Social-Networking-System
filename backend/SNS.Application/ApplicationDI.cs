using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SNS.Application.ContentManagement;
using SNS.Application.Identity.Shared.Services;
using SNS.Application.Profiles.Profiles;
using SNS.Application.Shared.Services;
using SNS.Application.Shared.Settings;

namespace SNS.Application;

public static class ApplicationDI
{
    public static IServiceCollection AddApplicationDI(
        this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddIdentityApplicationServices(configuration)
            .AddSharedApplicationServicesDI()
            .AddSettingsDI(configuration)
            .AddApplicationSocialGrcontentaph()
            .AddContentManagementApplicationDI();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ApplicationDI).Assembly);
        });

        return services;
    }
}
