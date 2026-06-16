using SNS.Domain.Projects.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Repositories;

public class ProjectMediaRepository : Repository<ProjectMedia>
{
    public ProjectMediaRepository(SNSDbContext context) : base(context) { }
}
