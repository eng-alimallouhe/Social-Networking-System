using Microsoft.Extensions.DependencyInjection;
using SNS.Application.Abstractions.Common;
using SNS.Application.Services.Common;

namespace SNS.Application.Shared.Services;

public static class SharedApplicationServicesDI
{
    public static IServiceCollection AddSharedApplicationServicesDI(
        this IServiceCollection services)
    {
        services.AddScoped<IGeneratorService, GeneratorService>();
        services.AddScoped<IHashingService, HashService>();

        return services;
    }
}
