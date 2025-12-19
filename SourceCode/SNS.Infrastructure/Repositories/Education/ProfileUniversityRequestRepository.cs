using SNS.Domain.Education.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Education;

public class ProfileUniversityRequestRepository : Repository<ProfileUniversityRequest>
{
    public ProfileUniversityRequestRepository(SNSDbContext context) : base(context) { }
}
