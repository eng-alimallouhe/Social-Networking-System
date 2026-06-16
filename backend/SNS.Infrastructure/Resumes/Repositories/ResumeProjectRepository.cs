using SNS.Domain.Resumes.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Resumes.Repositories;

public class ResumeProjectRepository : Repository<ResumeProject>
{
    public ResumeProjectRepository(SNSDbContext context) : base(context) { }
}
