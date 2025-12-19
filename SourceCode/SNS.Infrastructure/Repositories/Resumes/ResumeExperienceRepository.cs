using SNS.Domain.Resumes.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories;

public class ResumeExperienceRepository : Repository<ResumeExperience>
{
    public ResumeExperienceRepository(SNSDbContext context) : base(context) { }
}
