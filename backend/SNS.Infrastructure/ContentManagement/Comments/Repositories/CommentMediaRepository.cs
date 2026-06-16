using Microsoft.EntityFrameworkCore;
using SNS.Domain.ContentManagement.Comments.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.ContentManagement.Comments.Repositories;

public class CommentMediaRepository : Repository<CommentMedia>
{
    public CommentMediaRepository(SNSDbContext context) : base(context)
    {
    }
}
