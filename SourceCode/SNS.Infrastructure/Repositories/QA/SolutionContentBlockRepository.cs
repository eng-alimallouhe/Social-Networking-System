using SNS.Domain.QA.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.QA;

public class SolutionContentBlockRepository : Repository<SolutionContentBlock>
{
    public SolutionContentBlockRepository(SNSDbContext context) : base(context) { }
}