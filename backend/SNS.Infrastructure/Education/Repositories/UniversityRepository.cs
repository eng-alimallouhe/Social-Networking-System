using SNS.Domain.Educations.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Education.Repositories;

public class UniversityRepository : SoftDeletableRepository<University>
{
    public UniversityRepository(SNSDbContext context) : base(context) { }
}
