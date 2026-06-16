using Microsoft.EntityFrameworkCore;
using SNS.Domain.Jobs.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Repositories.Jobs;

public class CompanyAdministratorRepository : Repository<CompanyAdministrator>
{
    public CompanyAdministratorRepository(SNSDbContext context) : base(context)
    {
    }
}
