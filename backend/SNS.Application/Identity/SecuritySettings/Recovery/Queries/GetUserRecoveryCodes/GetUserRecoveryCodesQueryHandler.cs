using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using Microsoft.EntityFrameworkCore;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Identity.SecuritySettings.Recovery.Queries.GetUserRecoveryCodes;

public sealed record UserRecoveryCodesDto(
    int UsedCodesCount,
    int UnusedCodesCount,
    List<RecoveryCodeUsingSnapshot> RecoveryCodesUsingHistory
);

public sealed record RecoveryCodeUsingSnapshot(
    Guid CodeId,
    DateTime? UsedAt,
    DateTime GeneratingDate
);


public sealed record GetUserRecoveryCodesQuery(
) : IQuery<UserRecoveryCodesDto?>;


internal class GetUserRecoveryCodesQueryHandler
    : IQueryHandler<GetUserRecoveryCodesQuery, UserRecoveryCodesDto?>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;


    public GetUserRecoveryCodesQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result<UserRecoveryCodesDto?>> Handle(GetUserRecoveryCodesQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<UserRecoveryCodesDto?>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var userRecoveryCodes = await _dbContext.UsersSecuritySettings
            .Where(rc => rc.UserId == userId)
            .Select(rc => new UserRecoveryCodesDto
            (
                UsedCodesCount: rc.RecoveryCodes.Where(c => c.IsUsed).Count(),
                UnusedCodesCount: rc.RecoveryCodes.Where(c => !c.IsUsed).Count(),
                RecoveryCodesUsingHistory: rc.RecoveryCodes.Select(c => new RecoveryCodeUsingSnapshot
                (
                    c.Id,
                    c.UsedAt,
                    c.CreatedAt
                )).ToList()
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (userRecoveryCodes == null)
        {
            return Result<UserRecoveryCodesDto?>.Failure(ResourceStatusCode.NotFound);
        }

        return Result<UserRecoveryCodesDto?>.Success(userRecoveryCodes, ResourceStatusCode.Found);
    }
}
