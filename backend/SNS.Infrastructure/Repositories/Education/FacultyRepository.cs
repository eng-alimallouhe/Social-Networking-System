using SNS.Domain.Education.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Education;

public class FacultyRepository : SoftDeletableRepository<Faculty>
{
    public FacultyRepository(SNSDbContext context) : base(context) { }
}
