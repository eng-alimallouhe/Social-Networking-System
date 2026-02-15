using SNS.Domain.Content.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Posts;

public class PostMediaRepository : Repository<PostMedia>
{
    public PostMediaRepository(SNSDbContext context) : base(context) { }
}
