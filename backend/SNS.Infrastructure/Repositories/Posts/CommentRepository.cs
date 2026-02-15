using SNS.Domain.Content.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Posts;

public class CommentRepository : SoftDeletableRepository<Comment>
{
    public CommentRepository(SNSDbContext context) : base(context) { }
}
