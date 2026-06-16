using SNS.Domain.Identity.ArchiveManagement.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Identity.ArchiveManagement.Repositories;

public class IdentityArchiveRepository : Repository<IdentityArchive>
{
    public IdentityArchiveRepository(SNSDbContext context) : base(context) { }
}
