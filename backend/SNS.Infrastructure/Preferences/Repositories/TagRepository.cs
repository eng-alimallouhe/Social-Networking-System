using SNS.Domain.Preferences.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Preferences.Repositories;

public class TagRepository : Repository<Tag>
{
    public TagRepository(SNSDbContext context) : base(context) { }
}
