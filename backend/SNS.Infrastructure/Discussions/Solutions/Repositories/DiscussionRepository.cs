using SNS.Domain.Discussions.Solutions.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Repositories.QA;

public class DiscussionRepository : SoftDeletableRepository<Discussion>
{
    public DiscussionRepository(SNSDbContext context) : base(context) { }
}
