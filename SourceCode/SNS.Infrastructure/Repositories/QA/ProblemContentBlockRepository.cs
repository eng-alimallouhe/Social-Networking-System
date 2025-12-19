using SNS.Domain.QA.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.QA;

public class ProblemContentBlockRepository : Repository<ProblemContentBlock>
{
    public ProblemContentBlockRepository(SNSDbContext context) : base(context) { }
}
