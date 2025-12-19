using SNS.Domain.Resumes.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories;

public class ResumeRepository : SoftDeletableRepository<Resume>
{
    public ResumeRepository(SNSDbContext context) : base(context) { }
}
