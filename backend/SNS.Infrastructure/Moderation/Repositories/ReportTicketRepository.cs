using SNS.Domain.Moderation.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Moderation.Repositories;

public class ReportTicketRepository : Repository<ReportTicket>
{
    public ReportTicketRepository(SNSDbContext context) : base(context) { }
}
