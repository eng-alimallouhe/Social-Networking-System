using Microsoft.Extensions.DependencyInjection;
using SNS.Infrastructure.Services.Authentication;
using SNS.Infrastructure.Services.Cashing;
using SNS.Infrastructure.Services.Common;
using SNS.Infrastructure.Services.Loggings;
using SNS.Infrastructure.Services.Messaging;
using SNS.Infrastructure.Services.Security;

namespace SNS.Infrastructure.Services;

public static class InfrastructureServiceDI
{
    public static IServiceCollection AddInfrastructureService(
        this IServiceCollection services)
    {
        services
            .AddAuthenticationInfrastructureServices()
            .AddCommonInfrastructureServices()
            .AddMessaginService()
            .AddSecurityInfrastructureServices();
            
        return services;
    }
}
