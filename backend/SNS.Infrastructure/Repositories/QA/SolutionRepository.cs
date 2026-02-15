using SNS.Domain.QA.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.QA;

public class SolutionRepository : SoftDeletableRepository<Solution>
{
    public SolutionRepository(SNSDbContext context) : base(context) { }
}
