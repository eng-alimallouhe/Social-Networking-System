using SNS.Domain.Posts.Bridges;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Posts;

public class PostTopicRepository : Repository<PostTopic>
{
    public PostTopicRepository(SNSDbContext context) : base(context) { }
}
