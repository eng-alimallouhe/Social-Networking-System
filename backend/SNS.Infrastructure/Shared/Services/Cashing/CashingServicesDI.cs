using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SNS.Application.Abstractions.Caching;
using SNS.Application.ContentManagement.Communities.Services;
using SNS.Application.ContentManagement.Posts.Posts.Abstractions;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.abstractions;
using SNS.Application.Projects.Abstractions;
using StackExchange.Redis;

namespace SNS.Infrastructure.Shared.Services.Cashing;

public static class CashingServicesDI
{
    public static IServiceCollection AddCachingServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<
            ICacheService, RedisCacheService>();

        services.AddScoped<
            IProfileCacheKeyFactory, CacheKeyFactory>();
        
        services.AddScoped<
            IPostCacheKeyFactory, CacheKeyFactory>();
        
        services.AddScoped<
            IIdentityCacheKeyFactory, CacheKeyFactory>();
        
        services.AddScoped<
            ICommunityCacheKeyFactory, CacheKeyFactory>();

        services.AddScoped<
            IProjectCacheKeyFactory, CacheKeyFactory>();



        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var options = ConfigurationOptions.Parse(configuration.GetConnectionString("Redis") ?? "");
            options.AbortOnConnectFail = false; 
            options.ConnectRetry = 5;           
            options.SyncTimeout = 5000;         
            var multiplexer = ConnectionMultiplexer.Connect(options);
            
            Console.WriteLine($"Redis Connected: {multiplexer}");
            
            return multiplexer;
        });

        return services;
    }
}
