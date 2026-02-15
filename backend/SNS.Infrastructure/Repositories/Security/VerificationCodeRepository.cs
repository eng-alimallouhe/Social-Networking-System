using SNS.Domain.Security.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Security;

public class VerificationCodeRepository : Repository<VerificationCode>
{
    public VerificationCodeRepository(SNSDbContext context) : base(context) { }
}
