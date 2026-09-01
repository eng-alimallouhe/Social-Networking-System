using Microsoft.Extensions.DependencyInjection;
using SNS.Application.ContentManagement.Communities.Trending.Abstractions;
using SNS.Application.ContentManagement.Communities.Trending.Services;
using SNS.Application.ContentManagement.Posts.Posts.Abstractions;
using SNS.Application.ContentManagement.Posts.Posts.Services;

namespace SNS.Application.ContentManagement;

public static class ContentManagementApplicationDI
{
    public static IServiceCollection AddContentManagementApplicationDI(
        this IServiceCollection services)
    {
        services.AddScoped<IPostCacheService, PostCacheService>();
        services.AddScoped<IFeedBackgroundService, FeedBackgroundService>();
        services.AddScoped<IFeedFallbackService, FeedFallbackService>();
        services.AddScoped<ITrendingCommunityService, TrendingCommunityService>();

        return services;
    }
}
