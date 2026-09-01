using Microsoft.EntityFrameworkCore;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.ContentManagement.Communities.Memberships.Queries.IsMember;

/// <summary>
/// Handles the execution of <see cref="IsMemberQuery"/> to verify active membership.
/// </summary>
internal sealed class IsMemberQueryHandler : IQueryHandler<IsMemberQuery, bool>
{
    private readonly IApplicationDbContext _dbContext;

    public IsMemberQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<bool>> Handle(IsMemberQuery request, CancellationToken cancellationToken)
    {
        var isMember = await _dbContext.CommunityMemberships
            .AsNoTracking()
            .AnyAsync(m => m.CommunityId == request.CommunityId &&
                           m.MemberId == request.ProfileId &&
                           m.Status == CommunityMembershipStatus.Active, cancellationToken);

        return Result<bool>.Success(isMember, OperationStatusCode.Success);
    }
}
