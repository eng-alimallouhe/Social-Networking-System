using SNS.Domain.Resumes.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories;

public class ResumeEducationRepository : Repository<ResumeEducation>
{
    public ResumeEducationRepository(SNSDbContext context) : base(context) { }
}
