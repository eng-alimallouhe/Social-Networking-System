using SNS.Domain.Jobs.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Repositories.Jobs;

public class CompanyRepository : SoftDeletableRepository<Company>
{
    public CompanyRepository(SNSDbContext context) : base(context)
    {
    }
}
