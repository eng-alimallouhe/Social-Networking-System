using SNS.Domain.Preferences.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Preferences;

public class SkillsCategoryRepository : SoftDeletableRepository<SkillsCategory>
{
    public SkillsCategoryRepository(SNSDbContext context) : base(context) { }
}
