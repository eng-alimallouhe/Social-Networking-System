using SNS.Domain.Education.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Education;

public class UniversityRequestRepository : Repository<UniversityRequest>
{
    public UniversityRequestRepository(SNSDbContext context) : base(context) { }
}