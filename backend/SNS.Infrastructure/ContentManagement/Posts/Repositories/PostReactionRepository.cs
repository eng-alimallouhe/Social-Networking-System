using SNS.Domain.ContentManagement.Posts.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.ContentManagement.Posts.Repositories;

public class PostReactionRepository : Repository<PostReaction>
{
    public PostReactionRepository(SNSDbContext context) : base(context) { }
}
