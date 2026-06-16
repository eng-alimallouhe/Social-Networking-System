using SNS.Domain.Discussions.Solutions.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Repositories.QA;

public class SolutionRepository : SoftDeletableRepository<Solution>
{
    public SolutionRepository(SNSDbContext context) : base(context) { }
}
