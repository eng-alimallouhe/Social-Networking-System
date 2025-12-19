using SNS.Domain.Education.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Education;

public class ProfileFacultyRequestRepository : Repository<ProfileFacultyRequest>
{
    public ProfileFacultyRequestRepository(SNSDbContext context) : base(context) { }
}
