using SNS.Domain.Projects.Bridges;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories;

public class ProjectRatingRepository : Repository<ProjectRating>
{
    public ProjectRatingRepository(SNSDbContext context) : base(context) { }
}
