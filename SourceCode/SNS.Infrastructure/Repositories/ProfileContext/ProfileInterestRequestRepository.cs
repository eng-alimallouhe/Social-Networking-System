using SNS.Domain.ProfileContext.Bridges;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.ProfileContext;

public class ProfileInterestRequestRepository : Repository<ProfileInterestRequest>
{
    public ProfileInterestRequestRepository(SNSDbContext context) : base(context) { }
}
