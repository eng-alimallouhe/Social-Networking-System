using SNS.Domain.Identity.ArchiveManagement.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Identity.ArchiveManagement.Repositories;

public class ExportDataRequestRepository : Repository<ExportDataRequest>
{
    public ExportDataRequestRepository(SNSDbContext context) : base(context)
    {
    }
}
