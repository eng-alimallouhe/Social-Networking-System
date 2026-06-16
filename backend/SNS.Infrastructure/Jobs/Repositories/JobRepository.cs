using SNS.Domain.Jobs.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Repositories.Jobs;

public class JobRepository : SoftDeletableRepository<Job>
{
    public JobRepository(SNSDbContext context) : base(context) { }
}
