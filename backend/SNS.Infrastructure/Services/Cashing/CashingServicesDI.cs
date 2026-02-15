using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SNS.Application.Abstractions.Caching;
using SNS.Infrastructure.Services.Caching;
using StackExchange.Redis;

namespace SNS.Infrastructure.Services.Cashing;

public static class CashingServicesDI
{
    public static IServiceCollection AddCashingServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<
            ICacheService, RedisCacheService>();

        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(
                configuration.GetConnectionString("Redis") ?? ""));

        return services;
    }
}
