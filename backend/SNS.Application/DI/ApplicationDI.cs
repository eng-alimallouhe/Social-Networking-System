using Microsoft.Extensions.DependencyInjection;
using SNS.Application.Services.Authentication;
using SNS.Application.Services.Common;
using SNS.Application.Services.Security;

namespace SNS.Application.DI;

public static class ApplicationDI
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services
            .AddAuthenticationApplicationServices()
            .AddCommonApplicationServices()
            .AddSecurityApplicationServices();

        return services;
    }
}
