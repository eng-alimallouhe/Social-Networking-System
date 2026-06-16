using SNS.Domain.ContentManagement.Posts.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.ContentManagement.Posts.Repositories;

public class PostTopicRepository : Repository<PostTopic>
{
    public PostTopicRepository(SNSDbContext context) : base(context) { }
}
