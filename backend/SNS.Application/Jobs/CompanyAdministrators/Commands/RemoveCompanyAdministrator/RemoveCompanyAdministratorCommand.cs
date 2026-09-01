using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Jobs.Entities;
using SNS.Domain.Jobs.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Jobs;

namespace SNS.Application.Jobs.CompanyAdministrators.Commands.RemoveCompanyAdministrator;

public sealed record RemoveCompanyAdministratorCommand(
    Guid CompanyId,
    Guid TargetProfileId
) : ICommand;

internal sealed class RemoveCompanyAdministratorCommandHandler : ICommandHandler<RemoveCompanyAdministratorCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepository<CompanyAdministrator> _adminRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveCompanyAdministratorCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IRepository<CompanyAdministrator> adminRepository,
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _adminRepository = adminRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveCompanyAdministratorCommand request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;
        if (!currentProfileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var targetAdmin = await _dbContext.CompanyAdministrators
            .FirstOrDefaultAsync(ca => ca.CompanyId == request.CompanyId && ca.ProfileId == request.TargetProfileId, cancellationToken);

        if (targetAdmin == null)
        {
            return Result.Failure(CompanyAdministratorStatusCodes.AdminNotFound);
        }

        var isSelfRemoval = request.TargetProfileId == currentProfileId.Value;

        if (!isSelfRemoval)
        {
            var currentAdmin = await _dbContext.CompanyAdministrators
                .FirstOrDefaultAsync(ca => ca.CompanyId == request.CompanyId && ca.ProfileId == currentProfileId.Value, cancellationToken);

            if (currentAdmin == null || currentAdmin.AdminRole != CompanyRole.Owner)
            {
                return Result.Failure(CompanyAdministratorStatusCodes.NotCompanyAdmin);
            }
        }

        // If target is Owner, verify another Owner exists
        if (targetAdmin.AdminRole == CompanyRole.Owner)
        {
            var otherOwnersCount = await _dbContext.CompanyAdministrators
                .CountAsync(ca => ca.CompanyId == request.CompanyId && ca.ProfileId != request.TargetProfileId && ca.AdminRole == CompanyRole.Owner, cancellationToken);

            if (otherOwnersCount == 0)
            {
                return Result.Failure(CompanyAdministratorStatusCodes.CannotRemoveSoleOwner);
            }
        }

        _adminRepository.Delete(targetAdmin);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(CompanyAdministratorStatusCodes.AdminRemoved);
    }
}
