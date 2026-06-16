using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.ContentManagement.Posts.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;

namespace SNS.Infrastructure.ContentManagement.Posts.Repositories;

public static class PostsInfrastructureDI
{
    public static IServiceCollection AddPostsInfrastructureDI(
        this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<ISoftDeletableRepository<Post>, PostRepository>();
        services.AddScoped<IRepository<PostReaction>, PostReactionRepository>();
        services.AddScoped<IRepository<PostMedia>, PostMediaRepository>();
        services.AddScoped<IRepository<PostTag>, PostTagRepository>();
        services.AddScoped<IRepository<PostTopic>, PostTopicRepository>();
        services.AddScoped<ISoftDeletableRepository<PostView>, PostViewRepository>();
        services.AddScoped<IRepository<SavedPost>, SavedPostRepository>();

        // Services

        return services;
    }
}
