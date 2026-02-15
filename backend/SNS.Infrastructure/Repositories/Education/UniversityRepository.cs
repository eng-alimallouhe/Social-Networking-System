using SNS.Domain.Education.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Education;

public class UniversityRepository : SoftDeletableRepository<University>
{
    public UniversityRepository(SNSDbContext context) : base(context) { }
}
