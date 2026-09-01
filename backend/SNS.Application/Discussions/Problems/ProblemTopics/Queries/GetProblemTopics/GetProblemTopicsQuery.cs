using Microsoft.EntityFrameworkCore;
using SNS.Application.Discussions.Problems.ProblemTopics.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Discussions;

namespace SNS.Application.Discussions.Problems.ProblemTopics.Queries.GetProblemTopics;

/// <summary>
/// Query to retrieve AI-classified topics and confidence scores for a discussion problem.
/// </summary>
/// <param name="ProblemId">The unique identifier of the problem.</param>
public sealed record GetProblemTopicsQuery(Guid ProblemId) : IQuery<List<ProblemTopicDto>>;

/// <summary>
/// Handles <see cref="GetProblemTopicsQuery"/> to fetch problem topics.
/// </summary>
internal sealed class GetProblemTopicsQueryHandler : IQueryHandler<GetProblemTopicsQuery, List<ProblemTopicDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetProblemTopicsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<ProblemTopicDto>>> Handle(GetProblemTopicsQuery request, CancellationToken cancellationToken)
    {
        var problemExists = await _dbContext.Problems
            .AnyAsync(p => p.Id == request.ProblemId && p.IsActive, cancellationToken);

        if (!problemExists)
        {
            return Result<List<ProblemTopicDto>>.Failure(ProblemStatusCodes.ProblemNotFound);
        }

        var topics = await _dbContext.ProblemTopics
            .AsNoTracking()
            .Where(pt => pt.ProblemId == request.ProblemId)
            .Select(pt => new ProblemTopicDto(
                pt.TopicId,
                pt.Topic.Name,
                pt.Confidence))
            .ToListAsync(cancellationToken);

        return Result<List<ProblemTopicDto>>.Success(topics, OperationStatusCode.Success);
    }
}
