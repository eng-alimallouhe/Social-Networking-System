using Microsoft.Extensions.DependencyInjection;
using SNS.Infrastructure.ContentManagement.Comments;
using SNS.Infrastructure.ContentManagement.Communities.Repositories;
using SNS.Infrastructure.ContentManagement.Posts.Repositories;

namespace SNS.Infrastructure.ContentManagement;

public static class ContentManagementInfrastructureDI
{
    public static IServiceCollection AddContentManagementInfrastructureDI(this IServiceCollection services)
    {
        services.AddPostsInfrastructureDI();
        services.AddCommunitiesInfrastructureDI();
        services.AddCommentsInfrastructureDI();
        return services;
    }
}
