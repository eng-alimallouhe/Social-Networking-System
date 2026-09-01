using SNS.Domain.Jobs.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Repositories.Jobs;

public class CompanyCreateRequestRepository : Repository<CompanyCreateRequest>
{
    public CompanyCreateRequestRepository(SNSDbContext context) : base(context)
    {
    }
}
