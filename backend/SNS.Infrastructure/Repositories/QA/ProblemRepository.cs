using SNS.Domain.QA.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.QA;

public class ProblemRepository : SoftDeletableRepository<Problem>
{
    public ProblemRepository(SNSDbContext context) : base(context) { }
}
