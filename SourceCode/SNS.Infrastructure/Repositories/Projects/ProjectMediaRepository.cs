using SNS.Domain.Projects.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories;

public class ProjectMediaRepository : Repository<ProjectMedia>
{
    public ProjectMediaRepository(SNSDbContext context) : base(context) { }
}
