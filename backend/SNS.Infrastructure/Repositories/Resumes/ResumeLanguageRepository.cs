using SNS.Domain.Resumes.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories;

public class ResumeLanguageRepository : Repository<ResumeLanguage>
{
    public ResumeLanguageRepository(SNSDbContext context) : base(context) { }
}
