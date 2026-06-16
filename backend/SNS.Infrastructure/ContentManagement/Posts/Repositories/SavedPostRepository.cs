using Microsoft.EntityFrameworkCore;
using SNS.Domain.ContentManagement.Posts.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.ContentManagement.Posts.Repositories;

public class SavedPostRepository : Repository<SavedPost>
{
    public SavedPostRepository(SNSDbContext context) : base(context)
    {
    }
}
