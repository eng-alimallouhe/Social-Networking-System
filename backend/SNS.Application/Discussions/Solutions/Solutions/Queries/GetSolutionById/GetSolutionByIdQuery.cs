using Microsoft.EntityFrameworkCore;
using SNS.Application.Discussions.Solutions.Solutions.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Domain.Discussions.Shared.Enums;
using SNS.Domain.Discussions.Solutions.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Discussions;

namespace SNS.Application.Discussions.Solutions.Solutions.Queries.GetSolutionById;

/// <summary>
/// Query to retrieve complete details of a solution by its unique identifier.
/// </summary>
/// <param name="SolutionId">The unique identifier of the solution.</param>
public sealed record GetSolutionByIdQuery(Guid SolutionId) : IQuery<SolutionDetailsDto>;

/// <summary>
/// Handles <see cref="GetSolutionByIdQuery"/> to fetch solution details, author snapshot, content blocks, and current user vote status.
/// </summary>
internal sealed class GetSolutionByIdQueryHandler : IQueryHandler<GetSolutionByIdQuery, SolutionDetailsDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetSolutionByIdQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<SolutionDetailsDto>> Handle(GetSolutionByIdQuery request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;

        var solution = await _dbContext.Solutions
            .AsNoTracking()
            .Where(s => s.Id == request.SolutionId && s.IsActive)
            .Select(s => new
            {
                s.Id,
                s.ProblemId,
                s.Status,
                s.CreatedAt,
                s.UpdatedAt,
                AuthorId = s.AuthorId,
                AuthorProfile = _dbContext.Profiles
                    .Where(p => p.Id == s.AuthorId)
                    .Select(p => new
                    {
                        p.FullName,
                        p.Specialization,
                        p.ProfilePictureObjectKey
                    })
                    .FirstOrDefault(),
                ContentBlocks = s.ContentBlocks
                    .OrderBy(cb => cb.Order)
                    .Select(cb => new
                    {
                        cb.Id,
                        cb.Type,
                        cb.Content,
                        cb.ExtraInfo,
                        cb.Order
                    })
                    .ToList(),
                UpvotesCount = s.Votes.Count(v => v.Type == VoteType.Upvote),
                DownvotesCount = s.Votes.Count(v => v.Type == VoteType.Downvote),
                DiscussionsCount = s.Discussions.Count(d => d.IsActive),
                CurrentUserVote = currentProfileId.HasValue
                    ? s.Votes
                        .Where(v => v.VoterId == currentProfileId.Value)
                        .Select(v => (VoteType?)v.Type)
                        .FirstOrDefault()
                    : null
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (solution == null)
        {
            return Result<SolutionDetailsDto>.Failure(SolutionStatusCodes.SolutionNotFound);
        }

        var authorSnapshot = new ProfileSnapshotDto(
            Id: solution.AuthorId,
            FullName: solution.AuthorProfile?.FullName ?? string.Empty,
            Specialization: solution.AuthorProfile?.Specialization,
            ProfilePictureUrl: solution.AuthorProfile?.ProfilePictureObjectKey != null
                ? _fileStorageService.GetFilePublicUrl(solution.AuthorProfile.ProfilePictureObjectKey)
                : null);

        var contentBlocks = solution.ContentBlocks
            .Select(cb => new SolutionContentBlockDto(
                Id: cb.Id,
                Type: cb.Type,
                Content: cb.Type == SolutionBlockType.Media && !string.IsNullOrWhiteSpace(cb.Content)
                    ? _fileStorageService.GetFilePublicUrl(cb.Content)
                    : cb.Content,
                ExtraInfo: cb.ExtraInfo,
                Order: cb.Order))
            .ToList();

        var details = new SolutionDetailsDto(
            Id: solution.Id,
            ProblemId: solution.ProblemId,
            Status: solution.Status,
            CreatedAt: solution.CreatedAt,
            UpdatedAt: solution.UpdatedAt,
            Author: authorSnapshot,
            ContentBlocks: contentBlocks,
            UpvotesCount: solution.UpvotesCount,
            DownvotesCount: solution.DownvotesCount,
            DiscussionsCount: solution.DiscussionsCount,
            CurrentUserVote: solution.CurrentUserVote);

        return Result<SolutionDetailsDto>.Success(details, OperationStatusCode.Success);
    }
}
