using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.Jobs.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Jobs.CompanyAdministrators.Queries.GetMyCompanyAdministratorRole;

public sealed record GetMyCompanyAdministratorRoleQuery(Guid CompanyId) : IQuery<CompanyRole?>;

internal sealed class GetMyCompanyAdministratorRoleQueryHandler : IQueryHandler<GetMyCompanyAdministratorRoleQuery, CompanyRole?>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetMyCompanyAdministratorRoleQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result<CompanyRole?>> Handle(GetMyCompanyAdministratorRoleQuery request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;
        if (!currentProfileId.HasValue)
        {
            return Result<CompanyRole?>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var role = await _dbContext.CompanyAdministrators
            .AsNoTracking()
            .Where(ca => ca.CompanyId == request.CompanyId && ca.ProfileId == currentProfileId.Value)
            .Select(ca => (CompanyRole?)ca.AdminRole)
            .FirstOrDefaultAsync(cancellationToken);

        return Result<CompanyRole?>.Success(role, OperationStatusCode.Success);
    }
}
