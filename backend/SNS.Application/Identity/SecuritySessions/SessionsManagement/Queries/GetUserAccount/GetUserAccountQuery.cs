using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySessions.SessionsManagement.Queries.GetUserAccount;

public sealed record UserAccount(
    Guid Id,
    string UserName,
    string Email,
    string ProfilePictureUrl,
    DateTime LastPasswordChangedAt);

public sealed record GetUserAccountQuery(): IQuery<UserAccount>;

public sealed class GetUserAccountQueryHandler : IQueryHandler<GetUserAccountQuery, UserAccount>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;

    public GetUserAccountQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result<UserAccount>> Handle(GetUserAccountQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId == null)
        {
            return Result<UserAccount>.Failure(SecurityStatusCodes.AuthenticationRequired); ;
        }

        var userAccount = await _dbContext.Users
            .Where(u => u.Id == userId.Value)
            .Select(u => new UserAccount(
                u.Id,
                u.UserName,
                u.Email,
                u.UserProfile.ProfilePictureObjectKey!,
                u.LastPasswordChange))
            .FirstOrDefaultAsync(cancellationToken);

        if (userAccount == null)
        {
            return Result<UserAccount>.Failure(UserStatusCodes.NotFound);
        }

        return Result<UserAccount>.Success(userAccount, OperationStatusCode.Success);
    }
}