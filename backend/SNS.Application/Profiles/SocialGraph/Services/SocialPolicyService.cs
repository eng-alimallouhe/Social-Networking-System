using Microsoft.EntityFrameworkCore;
using SNS.Application.Profiles.SocialGraph.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Profiles;

namespace SNS.Application.Profiles.SocialGraph.Services;

public class SocialPolicyService : ISocialPolicyService
{
    private readonly IApplicationDbContext _dbContext;

    public SocialPolicyService(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> IsRelationshipAllowedAsync(Guid firstRelationshipPart, Guid secondRelationshipPart, CancellationToken cancellationToken = default)
    {
        var isSecondPartFound = await _dbContext
            .Profiles
            .AnyAsync(p => p.Id == secondRelationshipPart, cancellationToken);

        if (!isSecondPartFound)
        {
            return Result.Failure(ProfileStatusCodes.NotFound);
        }

        var isBlocked = await _dbContext.Blocks.AnyAsync(b =>
        (b.BlockerId == firstRelationshipPart && b.BlockedId == secondRelationshipPart) ||
        (b.BlockerId == secondRelationshipPart && b.BlockedId == firstRelationshipPart),
        cancellationToken);

        if (isBlocked)
        {
            return Result.Failure(ProfileStatusCodes.ProfilesBlockedEachOther);
        }

        return Result.Success(ProfileStatusCodes.RealtionClear);
    }
}
