using Microsoft.Extensions.DependencyInjection;
using SNS.Application.Abstractions.Common;

namespace SNS.Infrastructure.Services.Common;

public static class CommonInfrastructureServicesDI
{
    public static IServiceCollection AddCommonInfrastructureServices(
        this IServiceCollection services)
    {
        services.AddScoped<
            ICurrentUserService, CurrentUserService>();

        return services; 
    }
}