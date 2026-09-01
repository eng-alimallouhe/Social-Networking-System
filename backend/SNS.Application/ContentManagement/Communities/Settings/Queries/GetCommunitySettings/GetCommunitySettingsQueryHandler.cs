using Microsoft.EntityFrameworkCore;
using SNS.Application.ContentManagement.Communities.Settings.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.ContentManagement.Communities.Settings.Queries.GetCommunitySettings;

/// <summary>
/// Handles the execution of <see cref="GetCommunitySettingsQuery"/> to fetch settings for a community.
/// </summary>
internal sealed class GetCommunitySettingsQueryHandler : IQueryHandler<GetCommunitySettingsQuery, CommunitySettingsDto>
{
    private readonly IApplicationDbContext _dbContext;

    public GetCommunitySettingsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<CommunitySettingsDto>> Handle(GetCommunitySettingsQuery request, CancellationToken cancellationToken)
    {
        var communityExists = await _dbContext.Communities
            .AnyAsync(c => c.Id == request.CommunityId && c.IsActive, cancellationToken);

        if (!communityExists)
        {
            return Result<CommunitySettingsDto>.Failure(ResourceStatusCode.NotFound);
        }

        var settings = await _dbContext.CommunitySettings
            .AsNoTracking()
            .Where(s => s.CommunityId == request.CommunityId)
            .Select(s => new CommunitySettingsDto(
                s.CommunityId,
                s.AllowPostWithoutApproval,
                s.AllowInvitationsByMembers,
                s.AllowComments,
                s.AllowMediaUpload))
            .FirstOrDefaultAsync(cancellationToken);

        if (settings == null)
        {
            settings = new CommunitySettingsDto(request.CommunityId);
        }

        return Result<CommunitySettingsDto>.Success(settings, OperationStatusCode.Success);
    }
}
