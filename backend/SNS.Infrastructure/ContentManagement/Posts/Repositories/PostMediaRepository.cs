using SNS.Domain.ContentManagement.Posts.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.ContentManagement.Posts.Repositories;

public class PostMediaRepository : Repository<PostMedia>
{
    public PostMediaRepository(SNSDbContext context) : base(context) { }
}
