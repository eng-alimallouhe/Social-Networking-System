using SNS.Domain.Content.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Posts;

public class PostReactionRepository : Repository<PostReaction>
{
    public PostReactionRepository(SNSDbContext context) : base(context) { }
}
