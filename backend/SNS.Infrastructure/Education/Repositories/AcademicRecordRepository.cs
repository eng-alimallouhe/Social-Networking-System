using SNS.Domain.Educations.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Education.Repositories;

public class AcademicRecordRepository : Repository<AcademicRecord>
{
    public AcademicRecordRepository(SNSDbContext context) : base(context) { }
}
