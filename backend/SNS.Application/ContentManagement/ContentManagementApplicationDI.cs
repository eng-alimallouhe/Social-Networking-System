using Microsoft.Extensions.DependencyInjection;
using SNS.Application.ContentManagement.Posts.Abstractions;
using SNS.Application.ContentManagement.Posts.Services;

namespace SNS.Application.ContentManagement;

public static class ContentManagementApplicationDI
{
    public static IServiceCollection AddContentManagementApplicationDI(
        this IServiceCollection services)
    {
        services.AddScoped<IPostCacheService, PostCacheService>();
        services.AddScoped<IFeedBackgroundService, FeedBackgroundService>();
        services.AddScoped<IFeedFallbackService, FeedFallbackService>();


        return services;
    }
}
