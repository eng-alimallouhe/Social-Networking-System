using SNS.Domain.Projects.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories;

public class ProjectRepository : SoftDeletableRepository<Project>
{
    public ProjectRepository(SNSDbContext context) : base(context) { }
}
