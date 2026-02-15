using SNS.Domain.Preferences.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Preferences;

public class TagRepository : Repository<Tag>
{
    public TagRepository(SNSDbContext context) : base(context) { }
}
