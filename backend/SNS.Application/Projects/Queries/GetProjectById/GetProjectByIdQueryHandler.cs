using Microsoft.EntityFrameworkCore;
using SNS.Application.Projects.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using System.Linq;

namespace SNS.Application.Projects.Queries.GetProjectById;

internal sealed class GetProjectByIdQueryHandler : IQueryHandler<GetProjectByIdQuery, ProjectDetailsDto>
{
    private readonly IApplicationDbContext _dbContext;

    public GetProjectByIdQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<ProjectDetailsDto>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await _dbContext.Projects
            .Where(p => p.Id == request.ProjectId && p.IsActive)
            .Select(p => new ProjectDetailsDto(
                p.Id,
                p.OwnerId,
                p.Title,
                p.ShortDescription,
                p.MainImageUrl,
                p.ReadmeContent,
                p.GitHubUrl,
                p.LiveDemoUrl,
                p.Type,
                p.Status,
                p.IsActive,
                p.CreatedAt,
                p.UpdatedAt,
                p.Skills.Select(s => new ProjectSkillDto(s.SkillId, s.Skill.Name)).ToList(),
                p.Tags.Select(t => new ProjectTagDto(t.TagId, t.Tag.Name)).ToList(),
                p.Saves.Count(),
                p.Views.Count()
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (project == null)
        {
            return Result<ProjectDetailsDto>.Failure(ResourceStatusCode.NotFound);
        }

        return Result<ProjectDetailsDto>.Success(project, OperationStatusCode.Success);
    }
}
