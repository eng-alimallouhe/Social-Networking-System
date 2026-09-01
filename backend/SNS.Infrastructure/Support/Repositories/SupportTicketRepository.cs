using SNS.Domain.Support.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Support.Repositories;

public class SupportTicketRepository : Repository<SupportTicket>
{
    public SupportTicketRepository(SNSDbContext context) : base(context) { }
}
