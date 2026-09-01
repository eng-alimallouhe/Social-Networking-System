using Microsoft.EntityFrameworkCore;
using SNS.Application.Preferences.Skills.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;

namespace SNS.Application.Preferences.Skills.Queries.GetSkills;

internal sealed class GetSkillsQueryHandler : IQueryHandler<GetSkillsQuery, List<SkillDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetSkillsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<SkillDto>>> Handle(GetSkillsQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Skills.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(s => s.Name.ToLower().Contains(request.Search.ToLower()));
        }

        var skills = await query
            .OrderBy(s => s.Name)
            .Take(10)
            .Select(s => new SkillDto(s.Id, s.Name))
            .ToListAsync(cancellationToken);

        return Result<List<SkillDto>>.Success(skills, SNS.Shared.StatusCodes.OperationStatusCode.Success);
    }
}
