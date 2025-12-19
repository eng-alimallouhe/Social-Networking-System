using SNS.Domain.Posts.Bridges;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Posts;

public class PostTagRepository : Repository<PostTag>
{
    public PostTagRepository(SNSDbContext context) : base(context) { }
}
