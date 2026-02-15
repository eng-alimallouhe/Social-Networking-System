using SNS.Domain.Content.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Posts;

public class CommentReactionRepository : Repository<CommentReaction>
{
    public CommentReactionRepository(SNSDbContext context) : base(context) { }
}
