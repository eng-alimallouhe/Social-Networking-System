using SNS.Domain.Resumes.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Resumes.Repositories;

public class ResumeEducationRepository : Repository<ResumeEducation>
{
    public ResumeEducationRepository(SNSDbContext context) : base(context) { }
}
