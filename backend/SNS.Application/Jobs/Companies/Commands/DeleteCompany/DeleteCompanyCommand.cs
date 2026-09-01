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

namespace SNS.Application.Jobs.Companies.Commands.DeleteCompany;

public sealed record DeleteCompanyCommand(Guid CompanyId) : ICommand;

internal sealed class DeleteCompanyCommandHandler : ICommandHandler<DeleteCompanyCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly ISoftDeletableRepository<Company> _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCompanyCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        ISoftDeletableRepository<Company> companyRepository,
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;
        if (!currentProfileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var company = await _dbContext.Companies
            .FirstOrDefaultAsync(c => c.Id == request.CompanyId && c.IsActive, cancellationToken);

        if (company == null)
        {
            return Result.Failure(CompanyStatusCodes.CompanyNotFound);
        }

        var isOwner = await _dbContext.CompanyAdministrators
            .AnyAsync(ca => ca.CompanyId == request.CompanyId && ca.ProfileId == currentProfileId.Value && ca.AdminRole == CompanyRole.Owner, cancellationToken);

        if (!isOwner)
        {
            return Result.Failure(CompanyStatusCodes.NotCompanyAdmin);
        }

        _companyRepository.SoftDelete(company);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(CompanyStatusCodes.CompanyDeleted);
    }
}
