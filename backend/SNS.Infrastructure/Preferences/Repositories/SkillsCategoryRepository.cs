using SNS.Domain.Preferences.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Preferences.Repositories;

public class SkillsCategoryRepository : SoftDeletableRepository<SkillsCategory>
{
    public SkillsCategoryRepository(SNSDbContext context) : base(context) { }
}
