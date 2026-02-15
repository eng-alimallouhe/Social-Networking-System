using SNS.Domain.QA.Bridges;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.QA;

public class ProblemTagRepository : Repository<ProblemTag>
{
    public ProblemTagRepository(SNSDbContext context) : base(context) { }
}
