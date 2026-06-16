using Microsoft.EntityFrameworkCore;
using SNS.Domain.Identity.SecuritySettings.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Identity;

public class RecoveryCodeRepository : Repository<RecoveryCode>
{
    public RecoveryCodeRepository(SNSDbContext context) : base(context)
    {
    }
}
