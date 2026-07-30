using Microsoft.EntityFrameworkCore;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.Users.UsersManagement.Queries.checkUsernameAvailabilty;

/// <summary>
/// Represents a query to check whether a specific username is available for registration or change.
/// </summary>
/// <param name="UserName">The username to check for availability.</param>
public sealed record CheckUsernameAvailabiltyQuery(string UserName): IQuery<bool>;

/// <summary>
/// Handles the execution of <see cref="CheckUsernameAvailabiltyQuery"/> to verify username availability.
/// </summary>
/// <remarks>
/// Data retrieval and query logic:
/// 1. Queries the database to check if the requested username already exists.
/// 2. Returns a boolean indicating availability.
/// </remarks>
internal sealed class CheckUsernameAvailabiltyQueryHandler: IQueryHandler<CheckUsernameAvailabiltyQuery, bool>
{
    private readonly IApplicationDbContext _dbContext;
    public CheckUsernameAvailabiltyQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Result<bool>> Handle(CheckUsernameAvailabiltyQuery request, CancellationToken cancellationToken)
    {
        var isNotAvailable = await _dbContext.Users
            .AnyAsync(u => u.UserName == request.UserName, cancellationToken);

        if (isNotAvailable)
        {
            return Result<bool>.Failure(UserStatusCodes.UserNameAlreadyExists);
        }

        return Result<bool>.Success(true, UserStatusCodes.UserNameAvailable);
    }
}
