using SNS.Domain.Resumes.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Resumes.Repositories;

public class ResumeRepository : SoftDeletableRepository<Resume>
{
    public ResumeRepository(SNSDbContext context) : base(context) { }
}
