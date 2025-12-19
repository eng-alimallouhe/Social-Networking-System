using SNS.Domain.Security;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Security;

public class SupportTicketRepository : SoftDeletableRepository<SupportTicket>
{
    public SupportTicketRepository(SNSDbContext context) : base(context) { }
}
