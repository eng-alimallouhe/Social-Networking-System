using SNS.Domain.Support.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Support.Repositories;

public class TicketMessageAttachmentRepository : Repository<TicketMessageAttachment>
{
    public TicketMessageAttachmentRepository(SNSDbContext context) : base(context) { }
}
