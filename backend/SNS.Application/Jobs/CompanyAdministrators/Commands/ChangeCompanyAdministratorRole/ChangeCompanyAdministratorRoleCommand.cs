using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Jobs.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Jobs;

namespace SNS.Application.Jobs.CompanyAdministrators.Commands.ChangeCompanyAdministratorRole;

public sealed record ChangeCompanyAdministratorRoleCommand(
    Guid CompanyId,
    Guid TargetProfileId,
    CompanyRole NewRole
) : ICommand;

internal sealed class ChangeCompanyAdministratorRoleCommandHandler : ICommandHandler<ChangeCompanyAdministratorRoleCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeCompanyAdministratorRoleCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ChangeCompanyAdministratorRoleCommand request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;
        if (!currentProfileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var currentAdmin = await _dbContext.CompanyAdministrators
            .FirstOrDefaultAsync(ca => ca.CompanyId == request.CompanyId && ca.ProfileId == currentProfileId.Value, cancellationToken);

        if (currentAdmin == null || currentAdmin.AdminRole != CompanyRole.Owner)
        {
            return Result.Failure(CompanyAdministratorStatusCodes.NotCompanyAdmin);
        }

        var targetAdmin = await _dbContext.CompanyAdministrators
            .FirstOrDefaultAsync(ca => ca.CompanyId == request.CompanyId && ca.ProfileId == request.TargetProfileId, cancellationToken);

        if (targetAdmin == null)
        {
            return Result.Failure(CompanyAdministratorStatusCodes.AdminNotFound);
        }

        if (targetAdmin.AdminRole == CompanyRole.Owner && request.NewRole != CompanyRole.Owner)
        {
            var otherOwnersCount = await _dbContext.CompanyAdministrators
                .CountAsync(ca => ca.CompanyId == request.CompanyId && ca.ProfileId != request.TargetProfileId && ca.AdminRole == CompanyRole.Owner, cancellationToken);

            if (otherOwnersCount == 0)
            {
                return Result.Failure(CompanyAdministratorStatusCodes.CannotRemoveSoleOwner);
            }
        }

        targetAdmin.ChangeRole(request.NewRole);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(CompanyAdministratorStatusCodes.RoleChanged);
    }
}
