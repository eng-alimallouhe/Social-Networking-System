using SNS.Domain.Posts.Bridges;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Posts;

public class PostViewRepository : Repository<PostView>
{
    public PostViewRepository(SNSDbContext context) : base(context) { }
}