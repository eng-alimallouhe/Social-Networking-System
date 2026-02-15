using SNS.Domain.Education.Entities;
using SNS.Infrastructure.Data;
using SNS.Infrastructure.Repositories;

namespace SNS.Infrastructure.Repositories.Education;

// === Hard Delete ===
public class FacultyRequestRepository : Repository<FacultyRequest>
{
    public FacultyRequestRepository(SNSDbContext context) : base(context) { }
}
