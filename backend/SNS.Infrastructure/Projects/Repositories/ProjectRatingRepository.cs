using SNS.Domain.Projects.Bridges;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Repositories;

public class ProjectRatingRepository : Repository<ProjectRating>
{
    public ProjectRatingRepository(SNSDbContext context) : base(context) { }
}
