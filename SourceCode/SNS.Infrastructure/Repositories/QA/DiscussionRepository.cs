using SNS.Domain.QA.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.QA;

public class DiscussionRepository : SoftDeletableRepository<Discussion>
{
    public DiscussionRepository(SNSDbContext context) : base(context) { }
}
