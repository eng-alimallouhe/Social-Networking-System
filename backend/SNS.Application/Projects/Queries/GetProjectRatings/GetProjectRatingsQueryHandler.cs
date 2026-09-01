using Microsoft.EntityFrameworkCore;
using SNS.Application.Projects.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using System.Linq;

namespace SNS.Application.Projects.Queries.GetProjectRatings;

internal sealed class GetProjectRatingsQueryHandler : IQueryHandler<GetProjectRatingsQuery, Paged<ProjectRatingDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetProjectRatingsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Paged<ProjectRatingDto>>> Handle(GetProjectRatingsQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.ProjectRatings
            .Where(r => r.ProjectId == request.ProjectId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ProjectRatingDto(
                r.Id,
                r.RatingValue,
                r.Comment,
                r.CreatedAt,
                r.RaterId,
                r.Rater.FullName,
                r.Rater.Specialization,
                r.Rater.ProfilePictureObjectKey
            ));

        var count = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var paged = new Paged<ProjectRatingDto>(items, count, request.PageSize, request.Page);

        return Result<Paged<ProjectRatingDto>>.Success(paged, OperationStatusCode.Success);
    }
}
