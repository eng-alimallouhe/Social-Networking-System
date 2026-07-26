using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using Microsoft.EntityFrameworkCore;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Profiles.Profiles.Queries.GetSocialLinks;

public sealed class GetSocialLinksQueryHandler : IQueryHandler<GetSocialLinksQuery, SocialLinksDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    
    public GetSocialLinksQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }
    public async Task<Result<SocialLinksDto>> Handle(GetSocialLinksQuery request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }
        var profileSocialLinks = await _dbContext
            .Profiles
            .Where(p => p.Id == profileId.Value)
            .Select(p => new SocialLinksDto(
                p.GitHubUrl,
                p.LinkedInUrl,
                p.FacebookUrl,
                p.XUrl,
                p.Website))
            .FirstOrDefaultAsync(cancellationToken);

        if (profileSocialLinks == null)
        {
            return Result<SocialLinksDto>.Failure(ResourceStatusCode.NotFound);
        }

        return Result<SocialLinksDto>.Success(profileSocialLinks, ResourceStatusCode.Found);
    }
}