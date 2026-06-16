using SNS.Domain.Resumes.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Resumes.Repositories;

public class ResumeExperienceRepository : Repository<ResumeExperience>
{
    public ResumeExperienceRepository(SNSDbContext context) : base(context) { }
}
