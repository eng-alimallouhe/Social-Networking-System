using Microsoft.EntityFrameworkCore;
using SNS.Application.Jobs.JobSkills.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Jobs;

namespace SNS.Application.Jobs.JobSkills.Queries.GetJobSkills;

public sealed record GetJobSkillsQuery(Guid JobId) : IQuery<List<JobSkillDto>>;

internal sealed class GetJobSkillsQueryHandler : IQueryHandler<GetJobSkillsQuery, List<JobSkillDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetJobSkillsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<JobSkillDto>>> Handle(GetJobSkillsQuery request, CancellationToken cancellationToken)
    {
        var jobExists = await _dbContext.Jobs
            .AnyAsync(j => j.Id == request.JobId && j.IsActive, cancellationToken);

        if (!jobExists)
        {
            return Result<List<JobSkillDto>>.Failure(JobStatusCodes.JobNotFound);
        }

        var rawList = await (
            from js in _dbContext.JobSkills.AsNoTracking()
            join s in _dbContext.Skills.AsNoTracking() on js.SkillId equals s.Id
            where js.JobId == request.JobId
            select new
            {
                js.Id,
                js.JobId,
                js.SkillId,
                SkillName = s.Name
            }
        ).ToListAsync(cancellationToken);

        var items = rawList.Select(x => new JobSkillDto(
            Id: x.Id,
            JobId: x.JobId,
            SkillId: x.SkillId,
            SkillName: x.SkillName
        )).ToList();

        return Result<List<JobSkillDto>>.Success(items, OperationStatusCode.Success);
    }
}
