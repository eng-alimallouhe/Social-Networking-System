using SNS.Domain.Identity.ArchiveManagement.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Identity.ArchiveManagement.Repositories;

public class PasswordArchiveRepository : Repository<PasswordArchive>
{
    public PasswordArchiveRepository(SNSDbContext context) : base(context) { }
}
