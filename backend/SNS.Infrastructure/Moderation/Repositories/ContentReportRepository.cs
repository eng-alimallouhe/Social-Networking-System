using SNS.Domain.Moderation.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Moderation.Repositories;

public class ContentReportRepository : Repository<ContentReport>
{
    public ContentReportRepository(SNSDbContext context) : base(context) { }
}
