using SNS.Domain.ContentManagement.Comments.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.ContentManagement.Comments.Repositories;

public class CommentMentionRepository
    : Repository<CommentMention>
{
    public CommentMentionRepository(SNSDbContext context) : base(context)
    {
    }
}
