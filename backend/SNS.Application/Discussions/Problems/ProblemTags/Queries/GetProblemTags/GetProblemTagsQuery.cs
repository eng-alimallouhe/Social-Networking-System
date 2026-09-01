using Microsoft.EntityFrameworkCore;
using SNS.Application.Discussions.Problems.ProblemTags.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Discussions;

namespace SNS.Application.Discussions.Problems.ProblemTags.Queries.GetProblemTags;

/// <summary>
/// Query to retrieve all tags associated with a discussion problem.
/// </summary>
/// <param name="ProblemId">The unique identifier of the problem.</param>
public sealed record GetProblemTagsQuery(Guid ProblemId) : IQuery<List<ProblemTagDto>>;

/// <summary>
/// Handles <see cref="GetProblemTagsQuery"/> to fetch problem tags.
/// </summary>
internal sealed class GetProblemTagsQueryHandler : IQueryHandler<GetProblemTagsQuery, List<ProblemTagDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetProblemTagsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<ProblemTagDto>>> Handle(GetProblemTagsQuery request, CancellationToken cancellationToken)
    {
        var problemExists = await _dbContext.Problems
            .AnyAsync(p => p.Id == request.ProblemId && p.IsActive, cancellationToken);

        if (!problemExists)
        {
            return Result<List<ProblemTagDto>>.Failure(ProblemStatusCodes.ProblemNotFound);
        }

        var tags = await _dbContext.ProblemTags
            .AsNoTracking()
            .Where(pt => pt.ProblemId == request.ProblemId)
            .Select(pt => new ProblemTagDto(
                pt.TagId,
                pt.Tag.Name))
            .ToListAsync(cancellationToken);

        return Result<List<ProblemTagDto>>.Success(tags, OperationStatusCode.Success);
    }
}
