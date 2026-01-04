using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.Abstractions.Repositories;
using SNS.Domain.Content.Entities;
using SNS.Domain.Posts.Bridges;

namespace SNS.Infrastructure.Repositories.Posts;

public static class PostsDI
{
    public static IServiceCollection AddPostsRepositories(
        this IServiceCollection services)
    {
        // Soft Delete
        services.AddScoped<ISoftDeletableRepository<Post>, PostRepository>();
        services.AddScoped<ISoftDeletableRepository<Comment>, CommentRepository>();

        // Hard Delete
        services.AddScoped<IRepository<CommentReaction>, CommentReactionRepository>();
        services.AddScoped<IRepository<PostMedia>, PostMediaRepository>();
        services.AddScoped<IRepository<PostReaction>, PostReactionRepository>();
        services.AddScoped<IRepository<PostTag>, PostTagRepository>();
        services.AddScoped<IRepository<PostTopic>, PostTopicRepository>();
        services.AddScoped<IRepository<PostView>, PostViewRepository>();

        return services;
    }
}
