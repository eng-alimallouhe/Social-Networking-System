using Microsoft.EntityFrameworkCore;
using SNS.Application.ContentManagement.Communities.Communities.Contracts;
using SNS.Application.Discussions.Problems.Problems.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Domain.Discussions.Problems.Enums;
using SNS.Domain.Discussions.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Discussions;

namespace SNS.Application.Discussions.Problems.Problems.Queries.GetProblemById;

/// <summary>
/// Query to retrieve complete details of a discussion problem by its unique identifier.
/// </summary>
/// <param name="ProblemId">The unique identifier of the problem.</param>
public sealed record GetProblemByIdQuery(Guid ProblemId) : IQuery<ProblemDetailsDto>;

/// <summary>
/// Handles <see cref="GetProblemByIdQuery"/> to fetch problem details, author and community snapshots, structured content blocks, and current user vote status.
/// </summary>
internal sealed class GetProblemByIdQueryHandler : IQueryHandler<GetProblemByIdQuery, ProblemDetailsDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetProblemByIdQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<ProblemDetailsDto>> Handle(GetProblemByIdQuery request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;

        var problem = await _dbContext.Problems
            .AsNoTracking()
            .Where(p => p.Id == request.ProblemId && p.IsActive)
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.Status,
                p.Level,
                p.CreatedAt,
                p.UpdatedAt,
                AuthorId = p.Author.Id,
                AuthorFullName = p.Author.FullName,
                AuthorSpecialization = p.Author.Specialization,
                AuthorProfilePictureObjectKey = p.Author.ProfilePictureObjectKey,
                CommunityId = p.Community == null ? (Guid?)null : p.Community.Id,
                CommunityName = p.Community == null ? null : p.Community.Name,
                CommunityType = p.Community == null ? (SNS.Domain.ContentManagement.Communities.Enums.CommunityType?)null : p.Community.Type,
                CommunityLogoObjectKey = p.Community == null ? null : p.Community.LogoObjectKey,
                ContentBlocks = p.ContentBlocks
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
                Tags = p.ProblemTags.Select(pt => pt.Tag.Name).ToList(),
                Topics = p.ProblemTopics.Select(pt => pt.Topic.Name).ToList(),
                UpvotesCount = p.Votes.Count(v => v.Type == VoteType.Upvote),
                DownvotesCount = p.Votes.Count(v => v.Type == VoteType.Downvote),
                SolutionsCount = p.Solutions.Count(s => s.IsActive),
                ViewsCount = p.Views.Count(v => v.IsActive),
                CurrentUserVote = currentProfileId.HasValue
                    ? p.Votes
                        .Where(v => v.VoterId == currentProfileId.Value)
                        .Select(v => (VoteType?)v.Type)
                        .FirstOrDefault()
                    : null
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (problem == null)
        {
            return Result<ProblemDetailsDto>.Failure(ProblemStatusCodes.ProblemNotFound);
        }

        var authorSnapshot = new ProfileSnapshotDto(
            Id: problem.AuthorId,
            FullName: problem.AuthorFullName,
            Specialization: problem.AuthorSpecialization,
            ProfilePictureUrl: problem.AuthorProfilePictureObjectKey != null
                ? _fileStorageService.GetFilePublicUrl(problem.AuthorProfilePictureObjectKey)
                : null);

        var communitySnapshot = (problem.CommunityId.HasValue && problem.CommunityType.HasValue)
            ? new CommunitySnapshotDto(
                Id: problem.CommunityId.Value,
                Name: problem.CommunityName ?? string.Empty,
                Type: problem.CommunityType.Value,
                LogoUrl: problem.CommunityLogoObjectKey != null
                    ? _fileStorageService.GetFilePublicUrl(problem.CommunityLogoObjectKey)
                    : null)
            : null;

        var contentBlocks = problem.ContentBlocks
            .Select(cb => new ProblemContentBlockDto(
                Id: cb.Id,
                Type: cb.Type,
                Content: (cb.Type == ProblemBlockType.Image || cb.Type == ProblemBlockType.Video) && !string.IsNullOrWhiteSpace(cb.Content)
                    ? _fileStorageService.GetFilePublicUrl(cb.Content)
                    : cb.Content,
                ExtraInfo: cb.ExtraInfo,
                Order: cb.Order))
            .ToList();

        var details = new ProblemDetailsDto(
            Id: problem.Id,
            Title: problem.Title,
            Status: problem.Status,
            Level: problem.Level,
            CreatedAt: problem.CreatedAt,
            UpdatedAt: problem.UpdatedAt,
            Author: authorSnapshot,
            Community: communitySnapshot,
            ContentBlocks: contentBlocks,
            Tags: problem.Tags,
            Topics: problem.Topics,
            UpvotesCount: problem.UpvotesCount,
            DownvotesCount: problem.DownvotesCount,
            SolutionsCount: problem.SolutionsCount,
            ViewsCount: problem.ViewsCount,
            CurrentUserVote: problem.CurrentUserVote);

        return Result<ProblemDetailsDto>.Success(details, OperationStatusCode.Success);
    }
}
