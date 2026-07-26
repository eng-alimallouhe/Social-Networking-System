using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.ContentManagement.Comments.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Infrastructure.ContentManagement.Comments.Repositories;

namespace SNS.Infrastructure.ContentManagement.Comments;

public static class CommentsInfrastructureDI
{
    public static IServiceCollection AddCommentsInfrastructureDI(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<ISoftDeletableRepository<Comment>, CommentRepository>();
        services.AddScoped<IRepository<CommentReaction>, CommentReactionRepository>();
        services.AddScoped<IRepository<CommentMedia>, CommentMediaRepository>();
        services.AddScoped<IRepository<CommentMention>, CommentMentionRepository>();

        // Services

        return services;
    }
}
