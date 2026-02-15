using SNS.Domain.ProfileContext.Bridges;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.ProfileContext;

public class ProfileInterestRepository : Repository<ProfileInterest>
{
    public ProfileInterestRepository(SNSDbContext context) : base(context) { }
}
