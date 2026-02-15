using SNS.Domain.Jobs.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Jobs;

public class JobApplicationRepository : SoftDeletableRepository<JobApplication>
{
    public JobApplicationRepository(SNSDbContext context) : base(context) { }
}
