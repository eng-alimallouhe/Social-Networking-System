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

namespace SNS.Application.Jobs.CompanyAdministrators.Commands.AddCompanyAdministrator;

public sealed record AddCompanyAdministratorCommand(
    Guid CompanyId,
    Guid TargetProfileId,
    CompanyRole Role = CompanyRole.Manager
) : ICommand<Guid>;

internal sealed class AddCompanyAdministratorCommandHandler : ICommandHandler<AddCompanyAdministratorCommand, Guid>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepository<CompanyAdministrator> _adminRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddCompanyAdministratorCommandHandler(
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

    public async Task<Result<Guid>> Handle(AddCompanyAdministratorCommand request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;
        if (!currentProfileId.HasValue)
        {
            return Result<Guid>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var company = await _dbContext.Companies
            .FirstOrDefaultAsync(c => c.Id == request.CompanyId && c.IsActive, cancellationToken);

        if (company == null)
        {
            return Result<Guid>.Failure(CompanyStatusCodes.CompanyNotFound);
        }

        var currentAdmin = await _dbContext.CompanyAdministrators
            .FirstOrDefaultAsync(ca => ca.CompanyId == request.CompanyId && ca.ProfileId == currentProfileId.Value, cancellationToken);

        if (currentAdmin == null || currentAdmin.AdminRole != CompanyRole.Owner)
        {
            return Result<Guid>.Failure(CompanyAdministratorStatusCodes.NotCompanyAdmin);
        }

        var targetProfile = await _dbContext.Profiles
            .FirstOrDefaultAsync(p => p.Id == request.TargetProfileId && p.IsActive, cancellationToken);

        if (targetProfile == null)
        {
            return Result<Guid>.Failure(CompanyAdministratorStatusCodes.ProfileNotFound);
        }

        var exists = await _dbContext.CompanyAdministrators
            .AnyAsync(ca => ca.CompanyId == request.CompanyId && ca.ProfileId == request.TargetProfileId, cancellationToken);

        if (exists)
        {
            return Result<Guid>.Failure(CompanyAdministratorStatusCodes.AdminAlreadyExists);
        }

        var admin = CompanyAdministrator.Create(
            companyId: request.CompanyId,
            profileId: request.TargetProfileId,
            adminRole: request.Role);

        _adminRepository.Add(admin);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result<Guid>.Success(admin.Id, CompanyAdministratorStatusCodes.AdminAdded);
    }
}
