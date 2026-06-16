using SNS.Domain.Projects.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Repositories;

public class ProjectRepository : SoftDeletableRepository<Project>
{
    public ProjectRepository(SNSDbContext context) : base(context) { }
}
