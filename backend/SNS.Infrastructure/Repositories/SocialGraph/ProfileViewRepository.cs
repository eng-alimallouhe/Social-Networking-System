using SNS.Domain.SocialGraph.Bridges;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.SocialGraph;

public class ProfileViewRepository : SoftDeletableRepository<ProfileView>
{
    public ProfileViewRepository(SNSDbContext context) : base(context)
    {
    }
}