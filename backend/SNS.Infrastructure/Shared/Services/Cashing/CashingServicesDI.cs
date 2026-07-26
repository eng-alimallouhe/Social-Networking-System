using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SNS.Application.Abstractions.Caching;
using SNS.Application.ContentManagement.Communities.Services;
using SNS.Application.ContentManagement.Posts.Abstractions;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.abstractions;
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

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var options = ConfigurationOptions.Parse(configuration.GetConnectionString("Redis") ?? "");
            options.AbortOnConnectFail = false; // لا تنهار إذا Redis غير متاح عند الإقلاع
            options.ConnectRetry = 5;           // حاول إعادة الاتصال 5 مرات
            options.SyncTimeout = 5000;         // 5 ثواني Timeout للعمليات
            var multiplexer = ConnectionMultiplexer.Connect(options);
            
            Console.WriteLine($"Redis Connected: {multiplexer}");
            
            return multiplexer;
        });

        return services;
    }
}
