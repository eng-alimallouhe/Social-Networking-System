using Microsoft.EntityFrameworkCore;
using SNS.Application.ContentManagement.Communities.Communities.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.ContentManagement.Communities.Communities.Queries.GetCommunityById;

/// <summary>
/// Handles the execution of <see cref="GetCommunityByIdQuery"/> to retrieve community details and resolve temporary URLs.
/// </summary>
internal sealed class GetCommunityByIdQueryHandler : IQueryHandler<GetCommunityByIdQuery, CommunityDetailsDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetCommunityByIdQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<CommunityDetailsDto>> Handle(GetCommunityByIdQuery request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;

        var raw = await _dbContext.Communities
            .AsNoTracking()
            .Where(c => c.Id == request.CommunityId && c.IsActive)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Description,
                c.RulesText,
                c.Policy,
                c.Type,
                c.Status,
                c.LogoObjectKey,
                MembersCount = c.Memberships.Count(m => m.Status == CommunityMembershipStatus.Active),
                PostsCount = c.Posts.Count(p => p.IsActive),
                c.CreatedAt,
                c.UpdateAt,

                OwnerId = c.Owner.Id,
                OwnerFullName = c.Owner.FullName,
                OwnerSpecialization = c.Owner.Specialization,
                OwnerAvatarKey = c.Owner.ProfilePictureObjectKey,

                UserMembership = profileId != null
                    ? c.Memberships.Where(m => m.MemberId == profileId.Value && m.Status == CommunityMembershipStatus.Active).Select(m => (CommunityRole?)m.Role).FirstOrDefault()
                    : null,

                HasPendingRequest = profileId != null && _dbContext.CommunityJoinRequests.Any(r => r.CommunityId == c.Id && r.SubmitterId == profileId.Value && r.Status == JoinRequestStatus.Pending)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (raw == null)
        {
            return Result<CommunityDetailsDto>.Failure(ResourceStatusCode.NotFound);
        }

        var distinctKeys = new List<string?>
        {
            raw.LogoObjectKey,
            raw.OwnerAvatarKey
        }.Where(k => !string.IsNullOrWhiteSpace(k)).Distinct().ToList();

        var urlTasks = distinctKeys.Select(async k => new
        {
            Key = k!,
            Url = await _fileStorageService.GetTemporaryUrlAsync(k!, TimeSpan.FromHours(1))
        });
        var resolvedUrls = await Task.WhenAll(urlTasks);
        var urlMap = resolvedUrls.ToDictionary(r => r.Key, r => r.Url);

        var details = new CommunityDetailsDto(
            Id: raw.Id,
            Name: raw.Name,
            Description: raw.Description,
            RulesText: raw.RulesText,
            Policy: raw.Policy,
            Type: raw.Type,
            Status: raw.Status,
            LogoUrl: !string.IsNullOrWhiteSpace(raw.LogoObjectKey) && urlMap.TryGetValue(raw.LogoObjectKey, out var logoUrl) ? logoUrl : null,
            MembersCount: raw.MembersCount,
            PostsCount: raw.PostsCount,
            CreatedAt: raw.CreatedAt,
            UpdatedAt: raw.UpdateAt,
            Owner: new ProfileSnapshotDto(
                raw.OwnerId,
                raw.OwnerFullName,
                raw.OwnerSpecialization,
                !string.IsNullOrWhiteSpace(raw.OwnerAvatarKey) && urlMap.TryGetValue(raw.OwnerAvatarKey, out var avatarUrl) ? avatarUrl : null
            ),
            IsMember: raw.UserMembership.HasValue,
            CurrentUserRole: raw.UserMembership,
            HasPendingJoinRequest: raw.HasPendingRequest
        );

        return Result<CommunityDetailsDto>.Success(details, OperationStatusCode.Success);
    }
}
