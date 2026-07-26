using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using Microsoft.EntityFrameworkCore;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Identity.SecuritySettings.Queries.GetUserPasskeys;

internal sealed class GetUserPasskeysQueryHandler : IQueryHandler<GetUserPasskeysQuery, IEnumerable<PasskeyDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetUserPasskeysQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result<IEnumerable<PasskeyDto>>> Handle(
        GetUserPasskeysQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        // القراءة مباشرة من الـ DbContext بدون Tracking
        var result = await _dbContext.UserPasskeys
            .Where(p => p.UserId == userId)
            .Select(p => new
            {
                p.Id,
                p.CredentialId,
                p.DeviceName,
                p.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var passkeys = result.Select(p => new PasskeyDto(
            p.Id,
            Convert.ToBase64String(p.CredentialId),
            p.DeviceName,
            p.CreatedAt));

        return Result<IEnumerable<PasskeyDto>>.Success(passkeys, OperationStatusCode.Success);
    }
}