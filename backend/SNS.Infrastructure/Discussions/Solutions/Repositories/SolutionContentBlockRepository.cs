using SNS.Domain.Discussions.Solutions.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Repositories.QA;

public class SolutionContentBlockRepository : Repository<SolutionContentBlock>
{
    public SolutionContentBlockRepository(SNSDbContext context) : base(context) { }
}
