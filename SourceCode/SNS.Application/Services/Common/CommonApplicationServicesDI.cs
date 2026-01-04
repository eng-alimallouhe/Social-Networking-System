using Microsoft.Extensions.DependencyInjection;
using SNS.Application.Abstractions.Common;

namespace SNS.Application.Services.Common;

public static class CommonApplicationServicesDI
{
    public static IServiceCollection AddCommonApplicationServices(
        this IServiceCollection services)
    {
        services.AddScoped<
            IGeneratorService, GeneratorService>();

        services.AddScoped<
            IHashingService, HashService>();

        return services;
    }
}
