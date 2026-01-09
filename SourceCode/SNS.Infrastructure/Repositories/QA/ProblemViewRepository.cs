using SNS.Domain.QA.Bridges;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.QA;

public class ProblemViewRepository : SoftDeletableRepository<ProblemView>
{
    public ProblemViewRepository(SNSDbContext context) : base(context) { }
}
