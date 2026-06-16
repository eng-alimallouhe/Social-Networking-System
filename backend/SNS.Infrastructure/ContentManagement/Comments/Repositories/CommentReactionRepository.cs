using SNS.Domain.ContentManagement.Comments.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.ContentManagement.Comments.Repositories;

public class CommentReactionRepository : Repository<CommentReaction>
{
    public CommentReactionRepository(SNSDbContext context) : base(context) { }
}
