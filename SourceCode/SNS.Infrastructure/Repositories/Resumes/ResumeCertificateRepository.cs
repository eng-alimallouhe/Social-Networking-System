using SNS.Domain.Resumes.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories;

public class ResumeCertificateRepository : Repository<ResumeCertificate>
{
    public ResumeCertificateRepository(SNSDbContext context) : base(context) { }
}
