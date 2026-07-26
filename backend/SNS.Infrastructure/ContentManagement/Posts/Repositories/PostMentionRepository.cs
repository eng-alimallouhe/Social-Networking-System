using SNS.Domain.ContentManagement.Posts.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.ContentManagement.Posts.Repositories;

public class PostMentionRepository
    : Repository<PostMention>
{
    public PostMentionRepository(SNSDbContext context) : base(context)
    {
    }
}
