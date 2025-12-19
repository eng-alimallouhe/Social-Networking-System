using SNS.Domain.Content.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Posts;

public class PostRepository : SoftDeletableRepository<Post>
{
    public PostRepository(SNSDbContext context) : base(context) { }
}
