using SNS.Domain.Resumes.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories;

public class ResumeProjectRepository : Repository<ResumeProject>
{
    public ResumeProjectRepository(SNSDbContext context) : base(context) { }
}
