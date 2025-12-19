using SNS.Domain.Education.Entities;
using SNS.Infrastructure.Data;
using SNS.Infrastructure.Repositories;

namespace SNS.Infrastructure.Repositories.Education;

// === Soft Delete ===
public class FacultyRepository : SoftDeletableRepository<Faculty>
{
    public FacultyRepository(SNSDbContext context) : base(context) { }
}
