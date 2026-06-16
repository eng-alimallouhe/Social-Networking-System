using SNS.Domain.Resumes.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Resumes.Repositories;

public class ResumeCertificateRepository : Repository<ResumeCertificate>
{
    public ResumeCertificateRepository(SNSDbContext context) : base(context) { }
}
