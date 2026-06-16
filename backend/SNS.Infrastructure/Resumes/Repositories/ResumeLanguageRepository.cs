using SNS.Domain.Resumes.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Resumes.Repositories;

public class ResumeLanguageRepository : Repository<ResumeLanguage>
{
    public ResumeLanguageRepository(SNSDbContext context) : base(context) { }
}
