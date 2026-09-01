using SNS.Domain.Support.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Support.Repositories;

public class TicketMessageRepository : Repository<TicketMessage>
{
    public TicketMessageRepository(SNSDbContext context) : base(context) { }
}
