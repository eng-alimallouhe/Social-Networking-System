using Microsoft.EntityFrameworkCore;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.Users.UsersManagement.Queries.checkUsernameAvailabilty;

public sealed record CheckUsernameAvailabiltyQuery(string UserName): IQuery<bool>;

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
