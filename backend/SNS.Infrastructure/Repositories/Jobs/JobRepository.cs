using SNS.Domain.Jobs.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Jobs;

public class JobRepository : SoftDeletableRepository<Job>
{
    public JobRepository(SNSDbContext context) : base(context) { }
}
