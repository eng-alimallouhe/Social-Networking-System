using Microsoft.EntityFrameworkCore;
using SNS.Application.Projects.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Projects.Queries.GetProjectMilestones;

internal sealed class GetProjectMilestonesQueryHandler : IQueryHandler<GetProjectMilestonesQuery, List<ProjectMilestoneDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetProjectMilestonesQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<ProjectMilestoneDto>>> Handle(GetProjectMilestonesQuery request, CancellationToken cancellationToken)
    {
        var items = await _dbContext.ProjectMilestones
            .Where(m => m.ProjectId == request.ProjectId)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new ProjectMilestoneDto(
                m.Id,
                m.Title,
                m.Description,
                m.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return Result<List<ProjectMilestoneDto>>.Success(items, OperationStatusCode.Success);
    }
}
