using SNS.Domain.QA.Bridges;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.QA;

public class ProblemTopicRepository : Repository<ProblemTopic>
{
    public ProblemTopicRepository(SNSDbContext context) : base(context) { }
}
